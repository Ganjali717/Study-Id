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
    el: '#application-detail',
    data: {
        role: null,
        courses: [],
        appCourse: [],
        id: null,
        successId: null,
        firstName: null,
        lastName: null,
        created: null,
        updated: null,
        phone: null,
        course: [],
        someCourses: [],
        selectedCourses: [],
        tasks: [],
        eachTask:[],
        email: null,
        startDate: null,
        australianResident: null,
        success: false,
        loader: false,
        error: null,

        operationalItem: null,
        operationalLoader: false,
        operationalError: null
    },
    methods: {
        edit: function edit() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var data = {
                id: self.id,
                firstName: self.firstName,
                lastName: self.lastName,
                phone: self.phone,
                email: self.email,
                courses: self.course,
                australianResident: self.australianResident,
                startDate: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY')
            }
            axios.post('/applications/edit/', data).then(function (response) {
                self.success = true;
                self.loader = false;
                setTimeout(() => { self.success = false; }, 5000);
            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 3000);
            });
        },
        loadDictionaries: function loadDictionaries() {
            var self = this;
            axios.get('/applications/dictionaries/').then(function (response) {
                self.someCourses.splice(0, self.someCourses.length);
                self.course.splice(0, self.course.length);
                self.selectedCourses.splice(0, self.selectedCourses.length);
                self.courses = response.data.courses;
                self.appCourse = response.data.appCourse;
                self.tasks = response.data.tasks;
                for (let i = 0; i < self.tasks.length; i++)  {
                    if (self.id === self.tasks[i].applicationId) {
                        self.eachTask.push(self.tasks[i]);
                    }
                }
                for (let i = 0; i < self.appCourse.length; i++) {
                    if (self.id === self.appCourse[i].applicationId) {
                        self.someCourses.push(self.appCourse[i].courseId);
                    }
                }
                for (let i = 0; i < self.courses.length; i++) {
                    for (let j = 0; j < self.someCourses.length; j++) {
                        if (self.someCourses[j] === self.courses[i].id) {
                            self.selectedCourses.push(self.courses[i].title);
                        }
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
                self.email = model.Email;
                self.firstName = model.FirstName;
                self.lastName = model.LastName;
                self.created = model.Created;
                self.updated = model.Updated;
                self.phone = model.Phone;
                self.course = model.Courses;
                self.startDate = model.StartDate;
                self.australianResident = (model.AustralianResident = true) ? "Australia" : "Another";
            }
        },
        clearForm: function clearForm() {
            var self = this;
            self.course = [];
            self.email = null;
            self.australianResident = false;
            self.firstName = null;
            self.lastName = null;
            self.startDate = null;

        },
        removePopup: function removePopup(item) {
            var self = this;
            self.operationalItem = item;
            setTimeout(() => $("#remove-tasks-application").modal("show"), 100);
        },
        remove: function remove() {
            var self = this;
            if (self.operationalLoader === true) return;
            self.operationalLoader = true;
            axios.post('/tasks/remove/' + self.operationalItem.id).then(function (response) {
                var itemIndex = self.items.indexOf(self.operationalItem);
                self.items.splice(itemIndex, 1);
                $("#remove-tasks-application").modal("hide");
                self.operationalItem = null;
                self.operationalLoader = false;
            }).catch(function (error) {
                self.operationalError = error.response.data;
                self.operationalLoader = false;
                setTimeout(() => { self.error = null; }, 5000);
            });
        },
    },
    mounted: function mounted() {
        var self = this;
        self.loadDictionaries();
        self.initForm(window.model);
        self.role = window.role;
        $("body").keyup(function (event) {
            if (event.keyCode === 13) {
                self.create();
            }
        });
    }
});