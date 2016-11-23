app.service('sortOptionsService', [sortOptionsService]);

function sortOptionsService() {
    this.getSortOptions = function() {
        let sortOptions = [];

        sortOptions.push(new SortOption(1, "Price desc", "Price", sortDirection.descending));
        sortOptions.push(new SortOption(2, "Price asc", "Price", sortDirection.ascending));

        sortOptions.push(new SortOption(3, "Date desc", "DatePublished", sortDirection.descending));
        sortOptions.push(new SortOption(4, "Date asc", "DatePublished", sortDirection.ascending));

        sortOptions.push(new SortOption(5, "User desc", "UserName", sortDirection.descending));
        sortOptions.push(new SortOption(6, "User asc", "UserName", sortDirection.ascending));

        return sortOptions;
    }

    return this;
}