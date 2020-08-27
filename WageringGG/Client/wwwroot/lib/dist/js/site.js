//FUNCTIONS
window.copyToClipboard = async (text) => {
	await navigator.clipboard.writeText(text);
};

//BLAZOR
window.blazorCollapse = (id, action) => {
	$(id).collapse(action);
};
window.blazorModal = (id, action) => {
	$(id).modal(action);
};

window.blazorConfirm = (message) => {
	return confirm(message);
};

window.encryptSecret = (secret, code) => {
	var encrypted = CryptoJS.AES.encrypt(secret, code).toString();
	localStorage.setItem("EncryptedSecret", encrypted);
	return encrypted;
};

window.decryptSecret = async (code) => {
	var encrypted = localStorage.getItem("EncryptedSecret");
	if (!encrypted) {
		alert("Could not retrieve the encrypted secret.");
		return;
    }
	var decrypted = CryptoJS.AES.decrypt(encrypted, code);
	if (!decrypted) {
		alert("Could not decrypt the secret.");
		return;
    }
	await copyToClipboard(decrypted.toString(CryptoJS.enc.Utf8));
	alert("The secret has been copied to your clipboard.");
};

//STELLAR
window.signTransaction = (tx, secret, phrase) => {
	const clientKeys = StellarSdk.Keypair.fromSecret(secret);
	const transaction = StellarSdk.TransactionBuilder.fromXDR(tx, phrase);
	transaction.sign(clientKeys);
	return transaction.toXDR();
};

//NOTIFICATION
window.getNotification = (title, text) => {
	const icon = "https://jgz84g.dm.files.1drv.com/y4mmBczk2XUOMHGwQtzk2VP0m_COC6fHpbNC9eJ25LY5dgwy9BNQUuWo6jA9vHHVHqRPLF2WmkAaRpo06NxbTNb2Vqk9DcCEnrEFU6UuyVOjnD9RfDz1yoSdF7-t0DcF8JH9pUNQqLkkd8FrDnL4bc1K8nvX8tbAzC0kaU1YmleOmfyA6SR0JJODeIArZgs5sm9WzArPDdNrlMyAl0rPLuLKQ?width=512&height=512&cropmode=none";
	if (window.Notification) {
		// check if permission is already granted
		if (Notification.permission === 'granted') {
			// show notification here
			var notify = new Notification(title, {
				body: text,
				icon: icon,
			});
		} else {
			// request permission from user
			Notification.requestPermission().then(function (p) {
				if (p === 'granted') {
					// show notification here
					var notify = new Notification(title, {
						body: text,
						icon: icon,
					});
				}
			}).catch(function (err) {
				console.error(err);
			});
		}
	}
};