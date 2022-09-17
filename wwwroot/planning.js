function initTooltips() {
    $('[data-toggle="tooltip"]').tooltip();
}

function switchRevealView() {
    $('.container-reveal').toggleClass('hidden');
}

function hideUserInputView() {
    $('.username-container').addClass('hidden');
}

function storeUserState(userState) {
    localStorage.setItem('userState', userState);
}

function restoreUserState() {
    return localStorage.getItem('userState');
}
