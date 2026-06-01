function setLanguage(lang) {

    document.documentElement.lang = lang;

    document.querySelectorAll("[data-hu]").forEach(element => {
        element.textContent = element.getAttribute(
            lang === "hu" ? "data-hu" : "data-en"
        );
    });

    localStorage.setItem("language", lang);
}

window.onload = () => {
    setLanguage("hu");
};