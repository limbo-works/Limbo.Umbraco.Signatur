const template = document.createElement("template");
template.innerHTML = `
  <style>
    :host { display: block; font-family: inherit; }
    .empty, .error { padding: 10px 12px; border: 1px solid #d8d7d9; border-radius: 3px; background: #f9f7f7; }
    .error { border-color: #c63632; }
    table { width: 100%; border-collapse: collapse; margin-bottom: 12px; }
    th, td { padding: 8px 10px; border-bottom: 1px solid #e9e7e7; text-align: left; vertical-align: top; }
    th { width: 160px; color: #413659; font-weight: 600; }
    small, .muted { color: #817f85; }
    button { min-height: 32px; padding: 0 12px; border: 1px solid #817f85; border-radius: 3px; background: #fff; cursor: pointer; }
    pre { display: none; max-height: 520px; overflow: auto; margin-top: 12px; padding: 12px; border: 1px solid #d8d7d9; border-radius: 3px; background: #f9f7f7; white-space: pre-wrap; word-break: break-word; }
    :host([show-source]) pre { display: block; }
  </style>
  <div id="content"></div>
`;

function normalizeStoredValue(value) {
  if (!value) return "";
  if (typeof value !== "string") return String(value ?? "");
  return value.startsWith("_") ? value.substring(1) : value;
}

function parseStoredValue(value) {
  const raw = normalizeStoredValue(value).trim();
  if (!raw) return { job: null, raw: "", sourceType: "xml" };

  if (raw.startsWith("{") || raw.startsWith("[")) {
    return { job: mapJsonJob(JSON.parse(raw)), raw, sourceType: "json" };
  }

  return { job: parseXmlJob(raw), raw, sourceType: "xml" };
}

function parseXmlJob(xml) {
  const parser = new DOMParser();
  const doc = parser.parseFromString(xml, "application/xml");
  const parseError = doc.querySelector("parsererror");

  if (parseError) {
    throw new Error("Failed parsing XML value.");
  }

  const text = (...selectors) => {
    for (const selector of selectors) {
      const element = doc.querySelector(selector);
      const value = element?.textContent?.trim();
      if (value) return value;
    }
    return "";
  };

  const categories = splitCategories(text("categories", "category"));

  return {
    id: text("webAdId", "webadid", "id", "guid"),
    title: text("title"),
    category: text("category"),
    applicationUrl: text("applicationUrl", "applicationurl", "link"),
    published: text("pubDate", "publishDate", "published", "publishedDate"),
    expires: text("expDate", "expirationDate", "expires"),
    deadline: text("deadline"),
    categories
  };
}

function mapJsonJob(data) {
  if (!data) return null;

  const categories = [];
  if (Array.isArray(data.data)) {
    for (const item of data.data) {
      const title = localize(item?.title);
      if (title !== "Stillingskategori" || !Array.isArray(item.value)) continue;
      for (const value of item.value) {
        const label = localize(value?.title);
        if (label) categories.push(label);
      }
    }
  }

  return {
    id: data.jobId ?? data.webAdId ?? data.id ?? "",
    title: localize(data.title) || data.title || "",
    category: data.category || "",
    applicationUrl: data.applicationUrl || data.url || "",
    published: data.publishDate || data.published || data.created || "",
    expires: data.expirationDate || data.expDate || data.expires || "",
    deadline: data.deadlineUTC || data.deadlineUtc || data.deadline || "",
    categories
  };
}

function splitCategories(value) {
  if (!value) return [];
  return value
    .split(/;|,/g)
    .map(x => x.trim())
    .filter(Boolean);
}

function localize(value, preferredLocale = "da-DK") {
  if (!value) return "";

  const localizations = Array.isArray(value.localization) ? value.localization : [];
  const match = localizations.find(x => x.locale === preferredLocale) ?? localizations[0];

  return match?.value ?? value.value ?? value.title ?? String(value ?? "");
}

function formatDate(value) {
  if (!value) return "N/A";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return escapeHtml(value);

  const label = new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(date);
  const diffSeconds = Math.round((date.getTime() - Date.now()) / 1000);
  const units = [
    ["year", 31536000],
    ["month", 2592000],
    ["week", 604800],
    ["day", 86400],
    ["hour", 3600],
    ["minute", 60]
  ];
  const formatter = new Intl.RelativeTimeFormat(undefined, { numeric: "auto" });

  for (const [unit, seconds] of units) {
    if (Math.abs(diffSeconds) >= seconds) {
      return `${escapeHtml(label)} <small>(${escapeHtml(formatter.format(Math.round(diffSeconds / seconds), unit))})</small>`;
    }
  }

  return `${escapeHtml(label)} <small>(${escapeHtml(formatter.format(diffSeconds, "second"))})</small>`;
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function valueOrEmpty(value) {
  return value == null || value === "" ? "N/A" : escapeHtml(value);
}

function makeRows(job) {
  const categories = job?.categories?.length ? job.categories : splitCategories(job?.category);

  return [
    ["Job ID", valueOrEmpty(job?.id)],
    ["Titel", valueOrEmpty(job?.title)],
    ["Udgivet", formatDate(job?.published)],
    ["Udløber", formatDate(job?.expires)],
    ["Deadline", formatDate(job?.deadline)],
    ["Kategori", valueOrEmpty(job?.category)],
    [categories.length === 1 ? "Kategori" : "Kategorier", categories.length ? escapeHtml(categories.join(", ")) : "N/A"],
    ["Ansøgningslink", job?.applicationUrl ? `<a href="${escapeHtml(job.applicationUrl)}" target="_blank" rel="noopener noreferrer">${escapeHtml(job.applicationUrl)}</a>` : "N/A"]
  ];
}

class LimboUmbracoSignaturJobDataPropertyEditor extends HTMLElement {

  #value;
  #root;

  set value(value) {
    this.#value = value;
    this.#render();
  }

  get value() {
    return this.#value;
  }

  connectedCallback() {
    if (!this.#root) {
      this.#root = this.attachShadow({ mode: "open" });
      this.#root.appendChild(template.content.cloneNode(true));
    }
    this.#render();
  }

  #render() {
    if (!this.#root) return;

    const content = this.#root.getElementById("content");

    try {
      const { job, raw, sourceType } = parseStoredValue(this.#value);

      if (!job) {
        content.innerHTML = `<div class="empty muted">N/A</div>`;
        return;
      }

      const rows = makeRows(job)
        .filter(([, value], index) => index !== 6 || value !== "N/A")
        .map(([label, value]) => `<tr><th>${escapeHtml(label)}</th><td>${value === "N/A" ? `<span class="muted">N/A</span>` : value}</td></tr>`)
        .join("");

      const size = raw ? ` <small>(${(raw.length / 1024).toFixed(2)} KB)</small>` : "";
      const sourceLabel = sourceType === "json" ? "JSON" : "XML";

      content.innerHTML = `
        <table>${rows}</table>
        <button id="toggle" type="button">Show ${sourceLabel}${size}</button>
        <pre>${escapeHtml(raw)}</pre>
      `;

      content.querySelector("#toggle")?.addEventListener("click", () => {
        const showSource = this.toggleAttribute("show-source");
        content.querySelector("#toggle").innerHTML = `${showSource ? "Hide" : "Show"} ${sourceLabel}${size}`;
      });
    } catch (error) {
      content.innerHTML = `<div class="error">Could not parse Signatur job data: ${escapeHtml(error?.message ?? error)}</div>`;
    }
  }

}

customElements.define("limbo-umbraco-signatur-job-data-property-editor", LimboUmbracoSignaturJobDataPropertyEditor);
export { LimboUmbracoSignaturJobDataPropertyEditor as element };
