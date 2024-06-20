
export function initializeBackground() {
    VANTA.NET({
        el: "#animation",
        mouseControls: true,
        touchControls: true,
        gyroControls: false,
        minHeight: 200.00,
        minWidth: 200.00,
        scale: 1.00,
        scaleMobile: 1.00,
        backgroundColor: 0x0
    })
}


$(function () {
    initializeBackground();
});