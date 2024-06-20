var servicesAnchor = document.getElementById("services");

export function toggleDrawer(id, buttonId) {
    const drawer = document.getElementById(id);
    const button = document.getElementById(buttonId);

    const computedStyles = window.getComputedStyle(drawer);
    const drawerHeight = computedStyles.getPropertyValue("max-height");

    if (drawerHeight === "0px") {
        drawer.classList.add("open");
        drawer.scrollIntoView({ behavior: "smooth", block: "start", inline: "start" });
    }
    else {
        drawer.classList.remove("open");
        button.scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
    }

    drawer.addEventListener("transitionstart", function () {
        if (drawer.classList.contains("open")) {
            button.classList.add("close");
        }
    });

    drawer.addEventListener("transitionend", function () {
        if (!drawer.classList.contains("open") && button.classList.contains("close")) {
            button.classList.remove("close");
        }

        window.addEventListener("scrollend", function () {
            if (drawer) {
                const isInViewport = isElementInViewport(drawer);
                if (!isInViewport) {
                    drawer.classList.remove("open");
                    button.classList.remove("close");
                }
            }
        });
    });


    function isElementInViewport(el) {
        var rect = el.getBoundingClientRect();
        return (
            rect.top < window.innerHeight &&
            rect.bottom > 0 &&
            rect.left < window.innerWidth &&
            rect.right > 0
        );
    }
}

