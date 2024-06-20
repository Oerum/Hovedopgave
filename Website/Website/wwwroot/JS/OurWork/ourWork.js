let marqueeInstance;

export function initializeMarquee() {
    marqueeInstance = $('.marquee').marquee({
        duration: 25000,
        duplicated: false,
        gap: 50,
        direction: 'left',
        pauseOnHover: false
    });

    // Start a timer to periodically check and restart the marquee if needed
    setInterval(restartMarqueeIfNeeded, 5000); // Check every 5 seconds
}


$(function () {
    initializeMarquee();
});

// Add event listener to anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault(); // Prevent the default anchor behavior (navigating to a new location)

        // Get the target element to scroll to
        const targetId = this.getAttribute('href').substring(1); // Remove the '#' from the href attribute
        const targetElement = document.getElementById(targetId);

        // Scroll to the target element
        if (targetElement) {
            targetElement.scrollIntoView({
                behavior: 'smooth', // Smooth scrolling behavior
                block: 'start' // Scroll to the top of the target element
            });
        }

        // Restart the marquee if needed
        restartMarqueeIfNeeded();
    });
});

// Function to restart the marquee if it's not running
function restartMarqueeIfNeeded() {
    if (marqueeInstance && marqueeInstance.marquee('isPaused')) {
        marqueeInstance.marquee('resume');
    }
}
