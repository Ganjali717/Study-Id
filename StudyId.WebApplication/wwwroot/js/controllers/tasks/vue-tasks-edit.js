"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language;
Vue.component('v-select', VueSelect.VueSelect);
var app = new Vue({
    el: '#tasks-edit',
    data: {
        applications: [],
        application: null,
        appCourse:[],
        courses: [],
        course: [],
        appId: [],
        task:null,
        taskType: [
            { title: "Email", value: "email" },
            { title: "Phone Call", value: "phone" },
            { title: "Note", value: "note" }
        ],
        status: "Active",
        id: null,
        successId: null,
        success: false,
        loader: false,
        error: null,
        show: true,
        visible: null,
        dueDate: null,
        created: null,
        updated:null,
        email: [],
        phone: [],
        title:"",
        description: "",
        someCourses: [],
        selectedCourses: []
    },
    validations: {
        description: { required: required },
        title: { required: required },
        application: { required: required },
        task: { required: required }
    },
    methods: {
        create: function create() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var model = {
                title: self.title,
                status: "0",
                description: self.description,
                taskEmail: self.email[0],
                taskPhone: self.phone[0],
                applicationId: self.application,
                dueDate: typeof self.dueDate == "undefined" ? null : moment(self.dueDate).format('DD/MM/YYYY')
            }
            axios.post('/tasks/create', model).then(function (response) {
                self.success = true;
                self.successId = response.data.data.id;
                setTimeout(() => { self.success = false; }, 5000);
                self.loader = false;
                self.clearForm();
                self.$v.$reset();

            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 3000);
            });
        },
        edit: function edit() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var data = {
                id: self.id,
                title: self.title,
                status: "0",
                dueDate: typeof self.dueDate == "undefined" ? null : moment(self.dueDate).format('DD/MM/YYYY'),
                description: self.description,
                taskEmail: self.email[0],
                taskPhone: self.phone[0],
                applicationId: self.application
            }
            axios.post('/tasks/edit/', data).then(function (response) {
                self.success = true;
                self.loader = false;
                setTimeout(() => { self.success = false; }, 5000);
            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 3000);
            });
        },
        loadType: function loadType(value) {
            console.log(self.appId);
        },
        loadDictionaries: function loadDictionaries() {
            var self = this;
            axios.get('/tasks/dictionaries/').then(function (response) {
                self.applications = response.data.allApps;
                self.statuses = response.data.statuses;


            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        loadInput: function loadInput(id) {
            var self = this;
            console.log(id);
            axios.get('/tasks/dictionaries/').then(function (response) {
                self.email.splice(0, self.email.length);
                self.phone.splice(0, self.phone.length);
                self.applications = response.data.allApps;
                    for (let i = 0; i < self.applications.length; i++) {
                        if (id === self.applications[i].id) {
                            console.log(self.applications[i].email);
                            self.email.push(self.applications[i].email);
                            console.log(self.applications[i].phone);
                            self.phone.push(self.applications[i].phone);
                        }
                    }

            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        initForm: function initForm(model) {
            var self = this;
            if (model) {
                self.id = model.Id;
                self.title = model.Title;
                self.email = model.Email;
                self.status = model.Status;
                self.phone = model.Phone;
                self.application = model.ApplicationId;
                self.task = model.Task;
                self.course = model.Courses;
                self.description = model.Description;
                self.dueDate = moment(model.DueDate, "DD/MM/YYYY").toDate();
            }
        },
        clearForm: function clearForm() {
            var self = this;
            self.course = [];
            self.email = null;
            self.firstName = null;
            self.lastName = null;
            self.dueDate = null;
        }
    },
    mounted: function mounted() {
        var self = this;
        self.loadDictionaries();
        self.initForm(window.model);
    }
});