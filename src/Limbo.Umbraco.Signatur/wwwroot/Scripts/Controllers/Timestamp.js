angular.module("umbraco").controller("Limbo.Umbraco.Signatur.Timestamp.Controller", function ($scope) {

    const vm = this;

    if (!$scope.model.value) return;

    // Umbraco/Angular messes with the saved value if it looks like an ISO 8601 date (or other formats), so the saved
    // value is prefixed with an underscore  which we therefore needs to strip
    const value = $scope.model.value[0] === "_" ? $scope.model.value.substr(1) : $scope.model.value;

    // Parse the date
    const date = new Date(value);

    // Initialize a timestamp object for the view
    vm.timestamp = {
        date: date,
        comment: moment().to(date)
    };

});