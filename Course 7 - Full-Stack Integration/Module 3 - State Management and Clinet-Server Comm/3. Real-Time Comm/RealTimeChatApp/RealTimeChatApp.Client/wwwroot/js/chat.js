window.chatInterop = {
    scrollToBottom: function (selector) {
        const element = document.querySelector(selector);
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    }
};
