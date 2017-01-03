app.service('productsService', ['apiRootAddress', '$http', '$q', productsService]);

function productsService(apiRootAddress, $http, $q) {
    this.productsApiAdress = apiRootAddress + "Products";
    this.sessionProductName = "product/";

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
        let product = this.getProductFromSessionStorage(id);
        let deferred = $q.defer();
        if (product === null)
            return $http.get(this.productsApiAdress + "/" + id);
        else {
            deferred.resolve(product);
            return deferred.promise;
        }
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

    this.getProductsOfUser = function (username) {
        return $http.get(this.productsApiAdress + "/" + username);
    }

    this.setProductInSessionStorage = function (product) {
        let name = this.sessionProductName + product.data.id;
        sessionStorage.setItem(name, JSON.stringify(product));
    }

    this.getProductFromSessionStorage = function (id) {
        let name = this.sessionProductName + id;
        let product = sessionStorage.getItem(name);
        return JSON.parse(product);
    }

    return this;
}