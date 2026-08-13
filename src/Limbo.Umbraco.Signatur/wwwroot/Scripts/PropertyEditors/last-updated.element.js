const template = document.createElement("template");
template.innerHTML = `
  <style>
    :host { display: block; }
    .box { display: flex; flex-direction: column; gap: 2px; padding: 10px 12px; border: 1px solid #d8d7d9; border-radius: 3px; background: #f9f7f7; font-family: inherit; }
    .muted { color: #817f85; }
    .small { font-size: 12px; color: #817f85; }
  </style>
  <div class="box">
    <span id="value" class="muted">N/A</span>
    <span id="relative" class="small"></span>
  </div>
`;

function normalize(value) {
  if (typeof value !== "string") return value;
  return value.startsWith("_") ? value.substring(1) : value;
}

function formatDate(value) {
  const normalized = normalize(value);
  if (!normalized) return { label: "N/A", relative: "" };

  const date = new Date(normalized);
  if (Number.isNaN(date.getTime())) return { label: normalized, relative: "" };

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
      return { label, relative: formatter.format(Math.round(diffSeconds / seconds), unit) };
    }
  }

  return { label, relative: formatter.format(diffSeconds, "second") };
}

class LimboUmbracoSignaturLastUpdatedPropertyEditor extends HTMLElement {

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

    const formatted = formatDate(this.#value);
    const value = this.#root.getElementById("value");
    const relative = this.#root.getElementById("relative");

    value.textContent = formatted.label;
    value.classList.toggle("muted", formatted.label === "N/A");
    relative.textContent = formatted.relative;
  }

}

customElements.define("limbo-umbraco-signatur-last-updated-property-editor", LimboUmbracoSignaturLastUpdatedPropertyEditor);
export { LimboUmbracoSignaturLastUpdatedPropertyEditor as element };
