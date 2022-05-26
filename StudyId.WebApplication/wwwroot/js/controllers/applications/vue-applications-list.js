"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language,
    //duplicate = (value, vm) =>vm.map(function(x){return x.email}).indexOf(value.email)===vm.map(function(x){return x.email}).lastIndexOf(value.email);
    duplicate = (value, vm) => vm.length <= 1 || (vm.length > 1 && vm.map(function (x) { return x.email }).indexOf(value.email) === vm.map(function (x) { return x.email }).lastIndexOf(value.email));
Vue.component('v-select', VueSelect.VueSelect);
var app = new Vue({
    el: '#applications-list',
    data: {
        role: null,
        pagesBlocks: 1,
        pagesBlock: 1,
        pagesInPageBlock: 1,
        pageCurrent: 1,
        pages: [],
        itemsCount: 25,
        itemsCountSelect: [5, 10, 25, 50, 100],
        total: 0,
        courses: [],
        course: [],
        isAustralianItems: [
            { title: "All" },
            { title: "Australian", value: true },
            { title: "Overseas", value: false }
        ],
        isAustralian: null,
        statuses: [],
        status: "",
        listStatus:[],
        q: null,
        from: "",
        to: "",
        items: [],
        sortBy: null,
        sortAsc: false,
        error: null,
        loader: false,

        operationalItem: null,
        operationalLoader: false,
        operationalError: null
    },
    watch: {
        course: function (val) {
            var self = this;
            self.search();
        },
        isAustralian: function (val) {
            var self = this;
            self.search();
        },
        from: function (val) {
            var self = this;
            self.search();
        },
        to: function (val) {
            var self = this;
            self.search();
        },
        status: function (val) {
            var self = this;
            self.search();
        }
    },
    methods: {
        debounceSearch: _.debounce(function (e) {
            var self = this;
            if (self.loader === true) return;
            self.q = e.target.value;
            self.search();
        }, 1000),
        sortingBy: function sortingBy(column) {
            var self = this;
            if (self.loader === true) return;
            self.sortBy = column;
            self.sortAsc = !self.sortAsc;
            self.search();
        },
        search: function search() {
            var self = this;
            if (self.loader === true) return;
            self.loader = true;
            var data = {
                q: self.q,
                from: self.from,
                to: self.to,
                isAustralian: self.isAustralian,
                course: self.course,
                status: self.status,
                page: self.pageCurrent,
                take: self.itemsCount,
                orderBy: self.sortBy,
                orderAsc: self.sortAsc
            }
            axios.post('/applications/search/', data).then(function (response) {
                self.items = response.data.data;
                self.total = response.data.total;
                self.calculatePageCount(self.total);
                self.loader = false;
            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 5000);
            });

        },
        pageBlockShiftTop: function pageBlockShiftTop(num) {
            var self = this;
            num === -1 ? self.pagesBlock = 1 : self.pagesBlock = self.pagesBlocks;
        },
        pageBlockShift: function pageBlockShift(num) {
            var self = this;
            if ((self.pagesBlock === self.pagesBlocks && num === 1) || (self.pagesBlock === 1 && num === -1)) {
                return;
            }
            num === 1 ? self.pagesBlock++ : self.pagesBlock--;
        },
        setPage: function setPage(page) {
            var self = this;
            if (page === self.pageCurrent) return;
            self.pageCurrent = page;
            self.search();
        },
        calculatePageCount: function calculatePageCount(total) {
            var self = this;
            self.pages = [];
            var pagesCount = Math.ceil(total / self.itemsCount);
            for (var i = 0; i < pagesCount; i++) {
                var page = i + 1;
                self.pages.push(page);
            }
            self.pagesBlocks = Math.ceil(self.pages.length / self.pagesInPageBlock);
        },
        changeItemsCount: function changeItemsCount() {
            var self = this;
            self.pageCurrent = 1;
            self.pagesBlock = 1;
            self.calculatePageCount(self.total);
            self.search();
        },
        removePopup: function removePopup(item) {
            var self = this;
            self.operationalItem = item;
            setTimeout(() => $("#remove-application").modal("show"), 100);
        },
        remove: function remove() {
            var self = this;
            if (self.operationalLoader === true) return;
            self.operationalLoader = true;
            axios.post('/applications/remove/' + self.operationalItem.id).then(function (response) {
                var itemIndex = self.items.indexOf(self.operationalItem);
                self.items.splice(itemIndex, 1);
                $("#remove-application").modal("hide");
                self.operationalItem = null;
                self.operationalLoader = false;
            }).catch(function (error) {
                self.operationalError = error.response.data;
                self.operationalLoader = false;
                setTimeout(() => { self.error = null; }, 5000);
            });
        },
        changeStatus: function changeStatus(item) {
            var bodyFormData = new FormData();
            bodyFormData.append('status', item.status);
            axios.post('/applications/change-status/' + item.id, bodyFormData).then(function (response) {

            }).catch(function (error) {
                self.error = error.response.data;
                setTimeout(() => { self.error = null; }, 5000);
            });
        },
        loadDictionaries: function loadDictionaries() {
            var self = this;
            axios.get('/applications/dictionaries/').then(function (response) {
                self.courses = response.data.courses;
                self.statuses = response.data.statuses;
                self.statuses.unshift({ title: 'All', value: "null" });
                for (var i = 0; i < self.statuses.length; i++) {
                    self.listStatus.push(self.statuses[i]);
                }
                self.listStatus.shift();
            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        copy: function copy(item) {
            var copiedText = [];
            copiedText.push(item.firstName + " " + item.lastName);
            copiedText.push(item.email);
            copiedText.push(item.phone);
            copiedText.push(item.startDate);
            copiedText.push(item.courseName);
            copiedText.push(item.status);
            copiedText.push(item.created);
            navigator.clipboard.writeText(copiedText);
        }
       

    },
    mounted: function mounted() {
        var self = this;
        self.loadDictionaries();
        self.search();
        self.role = window.role;
    }
});