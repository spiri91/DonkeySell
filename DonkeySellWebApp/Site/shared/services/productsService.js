app.service('productsService', ['apiRootAddress', '$http', productsService]);

function productsService(apiRootAddress, $http) {
    this.productsApiAdress = apiRootAddress + "Products";

    this.queryProducts = function (query, take, skip, orderBy, sortDirection) {
        return $http({
            method: 'GET',
            url: this.productsApiAdress +
                "/query/" +
                query +
                "/" +
                take +
                "/" +
                skip +
                "/" +
                orderBy +
                "/" +
                sortDirection
        });
    }

    this.getProduct = function (id) {
        return $http.get(this.productsApiAdress + "/" + id);
    }

    this.deleteProduct = function (id, token) {
        return $http({
            method: 'DELETE',
            url: this.productsApiAdress + "/" + id,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.addProduct = function (product, token) {
        return $http({
            method: 'POST',
            url: this.productsApiAdress,
            headers: { 'Authorization': 'Bearer ' + token },
            data: product
        });
    }

    this.getProductsOfUser = function(username) {
        return $http.get(this.productsApiAdress + "/" + username);
    }

    return this;
}
