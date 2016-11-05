app.service('messagesService', ['apiRootAddress', '$http', messagesService]);

function messagesService(apiRootAddress, $http) {
    this.composeAddress = function(id) {
        return apiRootAddress+"Products/" + id + "/messages"; 
    }

    this.getMessagesForProduct = function(id) {
        let fullApiAddress = this.composeAddress(id);

        return $http.get(fullApiAddress).then(this.successHandler, this.errorHandler);
    }

    this.postMessageForProduct = function (message, token) {
        let fullApiAddress = this.composeAddress(message.productId);

        return $http({method:'POST', url: fullApiAddress, headers:{'Authorization':'Bearer '+ token}, data:message }).then(this.successHandler, this.errorHandler);
    }

    this.deleteMessage = function (productId, messageId, token) {
        let fullApiAddress = this.composeAddress(productId) + "/" + messageId;

        return $http({method:'DELETE', url: fullApiAddress, headers:{'Authorization':'Bearer '+ token}}).then(this.successHandler, this.errorHandler);
    }

    this.successHandler = function (data) {
        return data;
    }

    this.errorHandler = function (error) {
        console.log(error.message);
    }

    return this;
}