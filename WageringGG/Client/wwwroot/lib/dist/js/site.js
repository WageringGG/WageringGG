window.blazorCollapse = (id, action) => {
	$(id).collapse(action);
};
window.blazorModal = (id, action) => {
	$(id).modal(action);
};

window.StellarSdk = StellarSdk

window.signTransaction = (tx, secret, phrase) => {
	const clientKeys = StellarSdk.Keypair.fromSecret(secret);
	const transaction = StellarSdk.TransactionBuilder.fromXDR(tx, phrase);
	transaction.sign(clientKeys);
	return transaction.toXDR();
};