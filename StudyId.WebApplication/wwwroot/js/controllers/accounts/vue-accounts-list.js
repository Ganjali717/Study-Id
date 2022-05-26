"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language,
    //duplicate = (value, vm) =>vm.map(function(x){return x.email}).indexOf(value.email)===vm.map(function(x){return x.email}).lastIndexOf(value.email);
    isNotDuplicate = (value, vm) => vm.length<=1 || (vm.length>1 && vm.map(function(x){return x.email}).indexOf(value.email)===vm.map(function(x){return x.email}).lastIndexOf(value.email));
Vue.component('v-select', VueSelect.VueSelect);
var app = new Vue({
    el: '#accounts-list',
    data: {
        pagesBlocks: 1,
        pagesBlock: 1,
        pagesInPageBlock: 1,
        pageCurrent: 1,
        pages: [],
        itemsCount: 25,
        itemsCountSelect: [5, 10, 25, 50, 100],
        total: 0,
        listStatuses: [],
        listRole:[],

        q: null,
        role: null,
        status: null,
        roles: [],
        statuses: [],
        items: [],
        sortBy: null,
        sortAsc: false,
        error: null,
        loader: false,
        operationalItem: null,
        operationalLoader: false,
        operationalError: null,
        inviteLoader: false,
        inviteError: null,
        inviteSuccess: false,
        inviteRoles: [],
        inviteItems: []
    },
    watch: {
        role: function (val) {
            var self = this;
            self.search();
        },
        status: function (val) {
            var self = this;
            self.search();
        }
    },
    validations: {
        inviteItems: {
            $each: {
                isNotDuplicate,
                email: {
                    required,
                    email,
                    async exist(value) {
                        if (typeof value == "undefined" || value==="") return true;
                        if (!email(value)) return true;
                        const response = await fetch(`/accounts/exist`,
                            {
                                 method: 'POST',
                                 headers: {
                                     'Content-Type': 'application/json'
                                     // 'Content-Type': 'application/x-www-form-urlencoded',
                                 },
                                 redirect: 'follow', // manual, *follow, error
                                 referrerPolicy: 'no-referrer', // no-referrer, *client
                                 body: JSON.stringify({email:value})
                            });
                        return !Boolean(await response.json());
                    }
                }
            }
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
                role: self.role,
                status: self.status,
                page: self.pageCurrent,
                take: self.itemsCount,
                orderBy: self.sortBy,
                orderAsc: self.sortAsc
            }
            axios.post('/accounts/search/', data).then(function (response) {
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
        loadDictionaries: function loadDictionaries() {
            var self = this;
            axios.get('/accounts/dictionaries/').then(function (response) {
                self.roles = response.data.roles;
                self.roles.unshift({ title: "All" });
                self.inviteRoles = response.data.roles;
                self.statuses = response.data.statuses;
                self.statuses.pop();
                self.statuses.unshift({ title: "All" });
                var defaultRole = self.inviteRoles[self.inviteRoles.length - 1].value;
                self.inviteItems.push({ email: "", role: defaultRole });
                for (var i = 0; i < self.statuses.length; i++) {
                    self.listStatuses.push(self.statuses[i]);
                }
                for (var i = 0; i < self.roles.length; i++) {
                    self.listRole.push(self.roles[i]);
                }
                self.listRole.shift();
                self.listStatuses.shift();
            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        removePopup: function removePopup(item) {
            var self = this;
            self.operationalItem = item;
            setTimeout(() => $("#remove-account").modal("show"), 100);
        },
        remove: function remove() {
            var self = this;
            if (self.operationalLoader === true) return;
            self.operationalLoader = true;
            axios.post('/accounts/remove/' + self.operationalItem.id).then(function (response) {

                var itemIndex = self.items.indexOf(self.operationalItem);
                self.items.splice(itemIndex, 1);
                $("#remove-account").modal("hide");
                self.operationalItem = null;
                self.operationalLoader = false;
            }).catch(function (error) {
                self.operationalError = error.response.data;
                self.operationalLoader = false;
                setTimeout(() => { self.error = null; }, 5000);
            });
        },
        clearForm: function clearForm() {
            var self = this;
            let defaultRole = self.inviteRoles[self.inviteRoles.length - 1].value;
            self.inviteItems = [];
            self.inviteItems.push({ email: "", role:defaultRole  });
            self.$v.$reset();
        },
        invite: function invite() {
            var self = this;
            self.$v.$touch();
            if (self.$v.$invalid) {
                return;
            }
            self.inviteLoader = true;
            axios.post('/accounts/invite/', self.inviteItems).then(function (response) {
                self.inviteItems = [];
                self.inviteLoader = false;
                self.inviteSuccess = true;
                setTimeout(() => { self.inviteSuccess = false; }, 3000);
                self.search();

            }).catch(function (error) {
                self.inviteError = error.response.data;
                self.inviteLoader = false;
                setTimeout(() => { self.inviteError = null; }, 5000);
            });
            
        },
        addInvite: function addInvite() {
            var self = this;
            if (self.inviteLoader === true) return;
            let defaultRole = self.inviteRoles[self.inviteRoles.length - 1].value;
            self.inviteItems.push({ email: "", role:defaultRole  });
        },
        removeInvite: function removeInvite(item) {
            var self = this;
            if (self.inviteLoader === true) return;
            var index = self.inviteItems.indexOf(item);
            self.inviteItems.splice(index, 1);
        },
        changeStatus: function changeStatus(item) {
            var bodyFormData = new FormData();
            bodyFormData.append('status', item.status);
            axios.post('/accounts/change-status/' + item.id, bodyFormData).then(function (response) {

            }).catch(function (error) {
                self.error = error.response.data;
                setTimeout(() => { self.error = null; }, 5000);
            });
        },
        checkExisting:function checkExisting(item) {
            console.log(item.email);
            item.exist = true;
        },
        sleep: function sleep(ms) {
            return new Promise((resolve, reject) => setTimeout(resolve, ms));
        }
    },
    mounted: function mounted() {
        var self = this;
        self.loadDictionaries();
        self.search();
    }
});