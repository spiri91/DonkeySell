app.service('productsService', ['apiRootAddress', '$http', productsService]);

function productsService(apiRootAddress, $http) {
    this.productsApiAdress = apiRootAddress + "Products";

    this.queryProducts = function (query, take, skip, orderBy, sortDirection) {
        return $http({ method: 'GET', url: this.productsApiAdress + "/query/" + query + "/" + take + "/" + skip + "/"+orderBy+"/"+sortDirection })
            .then(this.successHandler, this.errorHandler);
    }

    this.getProduct = function (id) {
        return $http.get(this.productsApiAdress + "/" + id)
        .then(this.successHandler, this.errorHandler);
    }

    this.deleteProduct = function (id, token) {
        return $http({
            method: 'DELETE',
            url: this.productsApiAdress + "/" + id,
            headers: { 'Authorization': 'Bearer ' + token }
        })
            .then(this.successHandler, this.errorHandler);
    }

    this.addProduct = function (product, token) {
        return $http({ method: 'POST', url: this.productsApiAdress, headers: { 'Authorization': 'Bearer ' + token }, data: product })
            .then(this.successHandler, this.errorHandler);
    }

    this.getProductsOfUser = function(username) {
        return $http.get(this.productsApiAdress + "/" + username)
       .then(this.successHandler, this.errorHandler);
    }

    this.successHandler = function (data) {
        return data;
    }

    this.errorHandler = function (error) {
        console.log(error.message);
    }

    return this;
}
