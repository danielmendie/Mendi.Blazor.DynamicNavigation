
const target = document.getElementById("blazor-error-ui");

const observer = new MutationObserver((mutationsList) => {
    for (const mutation of mutationsList) {
        if (mutation.type === "attributes" && mutation.attributeName === "style") {
            const display = window.getComputedStyle(target).display;
            if (display !== "none") {
                // Blazor error UI is visible, remove style attribute from body
                document.querySelector("body").removeAttribute("style");
                const offcanvas = document.getElementById("custom-error-offcanvas");
                if (offcanvas && !offcanvas.classList.contains("show")) {
                    offcanvas.classList.add("show");
                }
                //document.getElementById("mobile-collapse").disabled = true;
            }
        }
    }
});

if (target) {
    observer.observe(target, { attributes: true, attributeFilter: ['style'] });
}

window.themeInterop = {
    getSystemTheme: function () {
        const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        return isDark ? 'dark' : 'light';
    }
};