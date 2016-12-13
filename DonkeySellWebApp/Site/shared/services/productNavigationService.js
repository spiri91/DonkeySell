app.service('productNavigationService', ['productsService', '$q', productNavigationService]);

function productNavigationService(productsService, $q) {
    this.query = "";
    this.skip = 0;
    this.sortBy = {};
    this.currentId = 0;
    this.productsIds = [];
    this.count = 0;
    this.defaultItemsPerPage = 10;

    this.setValues = function (query, skip, sortBy, count, productIds) {
        this.query = query;
        this.skip = skip;
        this.sortBy = sortBy;
        this.count = count;
        this.productsIds = productIds;
    }

    this.getNextProduct = function (id) {
        let currentIndex = this.productsIds.indexOf(id);

        if (currentIndex === this.productsIds.length - 1)
            return null;

        return this.productsIds[currentIndex + 1];

    }

    this.getPreviousProduct = function (id) {
        let currentIndex = this.productsIds.indexOf(id);
        if (currentIndex === 0)
            return null;

        return this.productsIds[currentIndex - 1];

    };

    this.getNextProducts = function () {
        this.skip += this.productsIds.length;
        return this.getProducts();
    };

    this.getPreviousProducts = function () {
        this.skip -= this.defaultItemsPerPage;
        if (this.skip < 0)
            this.skip = 0;

        return this.getProducts();
    };

    this.getProducts = function () {
        if (this.query)
            return productsService.queryProducts(this.query,
                this.defaultItemsPerPage,
                this.skip,
                this.sortBy.value,
                this.sortBy.sortDirection);
        else {
            let deferred = $q.defer();
            deferred.resolve();
            return deferred.promise;
        }
    };

    this.setProductsIds = function (ids, inFront) {
        for (let i = 0; i <= ids.length; ++i)
            inFront === false ? this.productsIds.push(ids[i]) : this.productsIds.unshift(ids[i]);
    }

    this.checkIfIdIsAllreadyAdded = function (id) {
        return this.productsIds.indexOf(id) >= 0;
    }

    return this;
};