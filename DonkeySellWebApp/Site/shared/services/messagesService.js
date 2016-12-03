app.service('messagesService', ['apiRootAddress', '$http', messagesService]);

function messagesService(apiRootAddress, $http) {
    this.composeAddress = function(id) {
        return apiRootAddress+"Products/" + id + "/Messages"; 
    }

    this.getMessagesForProduct = function(id) {
        let fullApiAddress = this.composeAddress(id);

        return $http.get(fullApiAddress);
    }

    this.postMessageForProduct = function (message, token) {
        let fullApiAddress = this.composeAddress(message.productId);

        return $http({
            method: 'POST',
            url: fullApiAddress,
            headers: { 'Authorization': 'Bearer ' + token },
            data: message
        });
    }

    this.deleteMessage = function (productId, messageId, token) {
        let fullApiAddress = this.composeAddress(productId) + "/" + messageId;

        return $http({ method: 'DELETE', url: fullApiAddress, headers: { 'Authorization': 'Bearer ' + token } });
    }

    return this;
}