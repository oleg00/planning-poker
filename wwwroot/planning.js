function initTooltips() {
    $('[data-toggle="tooltip"]').tooltip();
}

function switchRevealView() {
    $('.container-reveal').toggleClass('hidden');
}

function hideUserInputView() {
    $('.username-container').addClass('hidden');
}

function storeUserName(userName) {
    localStorage.setItem('userName', userName);
}

function restoreUserName() {
    return localStorage.getItem('userName');
}
