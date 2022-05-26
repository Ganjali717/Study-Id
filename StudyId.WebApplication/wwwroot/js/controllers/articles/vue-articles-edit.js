Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    _window$validators$la = _window$validators.language;
Vue.use(VoerroTagsInput);
Vue.use(Vue2Editor);
const emptyAreaOfEditor = '<p><br></p>';
const editorMustBeContainSmth = (value) => value != emptyAreaOfEditor;
Vue.component('v-select', VueSelect.VueSelect);
var app = new Vue({
    components: {
        "tags-input": VoerroTagsInput,
        Vue2Editor
    },
    el: '#article-edit',
    data: {
        categories: {
            error: null,
            loader: false,
            pagesBlocks: 1,
            pagesBlock: 1,
            pagesInPageBlock: 1,
            pageCurrent: 1,
            pages: [],
            itemsCount: 25,
            itemsCountSelect: [5, 10, 25, 50, 100],
            total: 0,
            items: [],
            name: null,
            id: null,
            route:null
        },
        id: null,
        loader: false,
        category: null,
        mainImage: null,
        error: null,
        publishOn: null,
        categoryId: null,
        title: null,
        route: null,
        body: null,
        description: null,
        tagsList: [],
        tags: [],
        imageTags:[],
        success:false,
        successId:null,
        isPermanent:false,
        status:"Published",
        statuses:[],
        croppOption: {
            previewImage: "",
            mainImage: ""
        },
    },
    validations: {
        mainImage: { required: required },
        category: { required: required },
        title: { required: required },
        route: { required: required },
        body: {
            required,
            editorMustBeContainSmth
        },
        description: { required: required }
    },
    methods: {
        categoriesDebounceSearch: _.debounce(function (e) {
            var self = this;
            if (self.loader === true) return;
            self.q = e.target.value;
            self.categoriesSearch();
        }, 1000),
        categoriesSortingBy: function sortingBy(column) {
            var self = this;
            if (self.categories.loader === true) return;
            self.categories.sortBy = column;
            self.categories.sortAsc = !self.categories.sortAsc;
            self.categoriesSearch();
        },
        categoriesSearch: function categoriesSearch() {
            var self = this;
            if (self.categories.loader === true) return;
            self.categories.loader = true;
            var data = {
                q: self.q,
                page: self.categories.pageCurrent,
                take: self.categories.itemsCount,
                orderBy: self.categories.sortBy,
                orderAsc: self.categories.sortAsc
            }
            axios.post('/categories/search/', data).then(function (response) {
                self.categories.items = response.data.data;
                self.categories.total = response.data.total;
                self.categoriesCalculatePageCount(self.categories.total);
                self.categories.loader = false;
            }).catch(function (error) {
                self.categories.error = error.response.data;
                self.categories.loader = false;
                setTimeout(() => { self.categories.error = null; }, 5000);
            });
        },
        categoriesCalculatePageCount: function categoriesCalculatePageCount(total) {
            var self = this;
            self.categories.pages = [];
            var pagesCount = Math.ceil(total / self.categories.itemsCount);
            for (var i = 0; i < pagesCount; i++) {
                var page = i + 1;
                self.categories.pages.push(page);
            }
            self.categories.pagesBlocks = Math.ceil(self.categories.pages.length / self.categories.pagesInPageBlock);
        },
        categoriesPageBlockShiftTop: function pageBlockShiftTop(num) {
            var self = this;
            num === -1 ? self.categories.pagesBlock = 1 : self.categories.pagesBlock = self.categories.pagesBlocks;
        },
        categoriesPageBlockShift: function pageBlockShift(num) {
            var self = this;
            if ((self.categories.pagesBlock === self.categories.pagesBlocks && num === 1) || (self.categories.pagesBlock === 1 && num === -1)) {
                return;
            }
            num === 1 ? self.categories.pagesBlock++ : self.categories.pagesBlock--;
        },
        categoriesSetPage: function setPage(page) {
            var self = this;
            if (page === self.categories.pageCurrent) return;
            self.categories.pageCurrent = page;
            self.categoriesSearch();
        },
        categoriesAdd: function categoriesAdd() {
            var self = this;
            self.categories.loader = true;
            var data = {
                Id: self.categories.id,
                Title: self.categories.name,
                Route:self.categories.route
            }
            axios.post('/categories/create/', data).then(function (response) {
                self.categories.items.unshift(response.data.data);
                self.categories.loader = false;
                self.categories.name = null;
                self.categories.id = null;
            }).catch(function (error) {
                self.categories.error = error.response.data;
                self.categories.loader = false;
                setTimeout(() => { self.categories.error = null; }, 3000);
            });
        },
        categoriesEdit: function categoriesEdit(item) {
            var self = this;
            Vue.set(item, 'loader', true);
            var data = {
                Id: item.id,
                Title: item.title,
                Route:item.route
            }
            axios.post('/categories/edit/', data).then(function (response) {
                item.edit = false;
                item.loader = false;
            }).catch(function (error) {
                item.loader = false;
            });
        },
        categoriesDelete: function categoriesDelete(item) {
            var self = this;
            Vue.set(item, 'loader', true);
            var data = {
                Id: item.id,
                Title: item.title
            }
            axios.post('/categories/delete/' + item.id, data).then(function (response) {
                var index = self.categories.items.indexOf(item);
                self.categories.items.splice(index, 1);
                item.edit = false;
                item.loader = false;
            }).catch(function (error) {
                item.loader = false;
            });
        },
        previewFileChange: function previewFileChange(e) {
            var files = e.target.files || e.dataTransfer.files;
            if (!files.length)
                return;
            this.loadCropp(files[0]);
        },
        mainFileChange: function mainFileChange(e) {
            var files = e.target.files || e.dataTransfer.files;
            if (!files.length)
                return;
            this.loadCroppMain(files[0]);
        },
        saveMain: function saveMain() {
            var self = this;
            const { canvas } = self.$refs.mainImageCrroper.getResult();
            self.mainImage = canvas.toDataURL();
            $("#crop-modal-main").modal("hide");
        },
        categoryOnChange(event) {
            var self = this;
            self.categoryId = event.target.value;
        },
        buildRoute: function () {
            var self = this;
            self.route = self.title.replace(/(?!\w|\s)./g, '')
                .replace(/\s+/g, ' ')
                .replace(/^(\s*)([\W\w]*)(\b\s*$)/g, '$2')
                .split(' ').join('-')
                .toLowerCase();
        },
        buildItemRoute: function (item) {
            item.route = item.title.replace(/(?!\w|\s)./g, '')
                .replace(/\s+/g, ' ')
                .replace(/^(\s*)([\W\w]*)(\b\s*$)/g, '$2')
                .split(' ').join('-')
                .toLowerCase();
        },
        buildCategoryRoute: function () {
            var self = this;
            self.categories.route = self.categories.name.replace(/(?!\w|\s)./g, '')
                .replace(/\s+/g, ' ')
                .replace(/^(\s*)([\W\w]*)(\b\s*$)/g, '$2')
                .split(' ').join('-')
                .toLowerCase();
        },
        create: async function create() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var model = {
                title: self.title,
                route: self.route,
                shortDescription: self.description,
                body: self.body,
                image: self.mainImage,
                tags: self.tags.map(tag => tag.value).join(','),
                imageTags: self.imageTags.map(tag => tag.value).join(','),
                categoryId: self.category,
                isPermanent:self.isPermanent,
                publishOn: typeof self.publishOn == "undefined" ? null: moment(self.publishOn).format('DD/MM/YYYY')
            }

            axios.post('/articles/create', model).then(function (response) {
                self.success = true;
                self.successId = response.data.data.id;
                setTimeout(() => { self.success = false; }, 5000);
                self.loader = false;
                self.clearForm();
                self.$v.$reset();
              
            }).catch(function (error) {
                self.error = error.response.data;
                setTimeout(() => { self.error = null; }, 3000);
            });
            await self.sleep(7 * 1000);
            window.location.href = "/articles/index/";
        },
        edit: async  function edit() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var model = {
                id:self.id,
                title: self.title,
                route: self.route,
                shortDescription: self.description,
                body: self.body,
                image: self.mainImage,
                tags: self.tags.map(tag => tag.value).join(','),
                imageTags: self.imageTags.map(tag => tag.value).join(','),
                categoryId: self.category,
                isPermanent:self.isPermanent,
                publishOn: typeof self.publishOn == "undefined" ? null: moment(self.publishOn).format('DD/MM/YYYY')
            }

            axios.post('/articles/edit', model).then(function (response) {
                self.success = true;
                self.loader = false;
                setTimeout(() => { self.success = false; }, 5000);
                window.location.href = "/articles/index/";
            }).catch(function (error) {
                self.error = error.response.data;
                setTimeout(() => { self.error = null; }, 3000);
            });
            await self.sleep(7 * 1000);

            window.location.href = "/articles/index/";
        },
        clearForm:function clearForm() {
            var self = this;
            self.category = null;
            self.title = null;
            self.route = false;
            self.previewImage = null;
            self.mainImage = null;
            self.body = "<p><br>  </p>";
            self.tags = [];
            self.imageTags = [];
            self.description = null;
            self.publishOn = null;

        },
        loadCropp(file) {
            var self = this;
            const blob = URL.createObjectURL(file);
            const reader = new FileReader();
            reader.onload = (e) => {
                self.croppOption = {
                    previewImage: blob
                };
            };
            reader.readAsArrayBuffer(file);
            $("#crop-modal").modal("show");
        },
        loadCroppMain(file) {
            var self = this;
            const blob = URL.createObjectURL(file);
            const reader = new FileReader();
            reader.onload = (e) => {
                self.croppOption = {
                    mainImage: blob,
                    maxWidth:1000,
                    maxHeight:500
                };
            };
            reader.readAsArrayBuffer(file);
            $("#crop-modal-main").modal("show");
        },
        loadDictionaries: function loadDictionaries() {
            var self = this;
            axios.get('/articles/dictionaries/').then(function (response) {
                self.tagsList = response.data.tags;
                self.statuses = response.data.statuses;
            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        initForm: function initForm(model) {
            var self = this;
            if (model) {
                self.id = model.Id;
                self.category = model.CategoryId;
                self.status = model.Status;
                self.title = model.Title;
                self.route = model.Route;
                self.body = model.Body;
                self.description = model.ShortDescription;
                self.mainImage = model.Image;
                self.previewImage = model.ImagePreview;
                self.isPermanent = model.IsPermanent,
                self.publishOn =model.PublishOn!=null ? moment(model.PublishOn,"DD/MM/YYYY").toDate() : null;
                self.tags = model.Tags != null ? model.Tags.split(',').map(function(x){return {key:x,value:x}}) : [];
                self.imageTags = model.ImageTags != null ? model.ImageTags.split(',').map(function(x){return {key:x,value:x}}) : [];
                //self.australianResident = model.AustralianResident;

            }
        },
        sleep: function sleep(ms) {
            return new Promise((resolve, reject) => setTimeout(resolve, ms));
        }
    },
    mounted: function mounted() {
        var self = this;
        self.categoriesSearch();
        self.loadDictionaries();
        self.initForm(window.model);
    }
});