mergeInto(LibraryManager.library, {
  /*MessageToWeb: function (message, param) {
    window["GetMessageFromUnity"](UTF8ToString(message), UTF8ToString(param));
  },*/
  CloseBrowserTab: function () {
	window.close();
  }

  CopyToWebClipboard: function (text) {
    if(navigator.clipboard)
	{
		navigator.clipboard.writeText(UTF8ToString(text)).then(function() {
			console.log('Text copied to clipboard successfully!');
		}).catch(function(err) {
			console.error('Could not copy text: ', err);
			console.log(text);
		});
	}
	else
	{
		// navigator.clipboard is available only in secure contexts
		let textArea = document.createElement("textarea");
        textArea.value = UTF8ToString(text);
        // Avoid scrolling to bottom
        textArea.style.top = "0";
        textArea.style.left = "0";
        textArea.style.position = "fixed";
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        try {
            let successful = document.execCommand('copy');
            let msg = successful ? 'Text copied to clipboard successfully!' : 'Could not copy text';
            console.log(msg);
        } catch (err) {
            console.error('Could not copy text: ', err);
            console.log(text);
        }
        document.body.removeChild(textArea);
	}
  }
});