function initTooltips() {
    $('[data-toggle="tooltip"]').tooltip();
}

function switchRevealView() {
    $('.main-container').toggleClass('d-none');
    $('.container-reveal').toggleClass('d-none');
}

function hideUserInputView() {
    $('.username-container').addClass('d-none');
}

function storeUserName(userName) {
    localStorage.setItem('userName', userName);
}

function restoreUserName() {
    return localStorage.getItem('userName');
}
