export function scrollToTop() {
    const button = document.querySelector('.scroll-to-top-btn');
    button.addEventListener('click', function () {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });

    window.addEventListener('scroll', function () {
        button.classList.toggle('show', window.scrollY > 200);
    });
}

$(function () {
    scrollToTop();
});