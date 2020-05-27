window.blazorCollapse = (id, action) => {
    $(id).collapse(action);
};
window.blazorModal = (id, action) => {
    $(id).modal(action);
};

window.readChallengeTx = (challengeTx, serverAccountId, networkPassphrase) => {
    return StellarSdk.Utils.readChallengeTx(challengeTx, serverAccountId, networkPassphrase);
};