// making the filter panel hide and show

const hidePanelButton = document.getElementById('hideFilterPanel');
const showPanelButton = document.getElementById('showFilterPanel');

let panelIsHidden = sessionStorage.getItem('searchMenuHidden');

if (panelIsHidden === 'true') {
    searchMenu.classList.add('collapse');
}

hidePanelButton.addEventListener('click', function () {
    const searchMenu = document.getElementById('searchMenu');
    searchMenu.classList.add('collapse');

    sessionStorage.removeItem('searchMenuHidden');
    sessionStorage.setItem('searchMenuHidden', 'true');
});

showPanelButton.addEventListener('click', function () {
    const searchMenu = document.getElementById('searchMenu');
    searchMenu.classList.remove('collapse');

    sessionStorage.removeItem('searchMenuHidden');
    sessionStorage.setItem('searchMenuHidden', 'false');
});

// we want to highlight the button that corresponds to the search we are doing

const uri = window.location.href;
const posn = uri.indexOf('=');

if (!isNaN(posn)) {
    // retrieve the string following the '='
    const searchValue = uri.slice(posn + 1);

    // obtain button id by prefixing and removing any plus chars
    const buttonId = 'btn' + searchValue.split('+').join('');

    // get the button
    const theButton = document.getElementById(buttonId);

    // update the class for the button to make it stand out
    if (theButton != null) {
        theButton.classList.replace('btn-info', 'btn-primary');
    }
}

// the search reset button

const resetFilterButton = document.getElementById('btnResetFilter');

resetFilterButton.addEventListener('click', function () {
    document.getElementById("frmFilters").submit();
});

// todo: work out how to send proper querystring parms here