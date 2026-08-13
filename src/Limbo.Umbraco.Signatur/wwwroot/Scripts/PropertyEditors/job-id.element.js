const template = document.createElement("template");
template.innerHTML = `
  <style>
    :host { display: block; }
    .box { display: inline-flex; align-items: center; min-height: 32px; padding: 0 12px; border: 1px solid #d8d7d9; border-radius: 3px; background: #f9f7f7; font-family: inherit; }
    .muted { color: #817f85; }
  </style>
  <div class="box"><span id="value" class="muted">N/A</span></div>
`;

class LimboUmbracoSignaturJobIdPropertyEditor extends HTMLElement {

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
    const element = this.#root.getElementById("value");
    const value = this.#value ?? "";
    element.textContent = value === "" ? "N/A" : value;
    element.classList.toggle("muted", value === "");
  }

}

customElements.define("limbo-umbraco-signatur-job-id-property-editor", LimboUmbracoSignaturJobIdPropertyEditor);
export { LimboUmbracoSignaturJobIdPropertyEditor as element };
