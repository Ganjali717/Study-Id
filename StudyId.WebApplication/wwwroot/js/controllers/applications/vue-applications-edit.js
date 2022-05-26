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
    el: '#application-edit',
    data: {
        courses: [
            //{ title: "Business", value: "Business" },
            //{ title: "Digital Media & Information Technology", value: "Digital Media & Information Technology" },
            //{ title: "Early Childhood Education & Care", value: "Early Childhood Education & Care" },
            //{ title: "English", value: "English" },
            //{ title: "Environment Management & Sustainability", value: "Environment Management & Sustainability" },
            //{ title: "Hospitality & Culinary", value: "Hospitality & Culinary" },
            //{ title: "Leadership & Management", value: "Leadership & Management" },
            //{ title: "Marine Habitat Conservation & Restoration", value: "Marine Habitat Conservation & Restoration" },
            //{ title: "Marketing & Communications", value: "Marketing & Communications" }
        ],
        id: null,
        successId: null,
        firstName: null,
        lastName: null,
        phone: null,
        course: null,
        email: null,
        startDate: null,
        australianResident: false,
        success: false,
        loader: false,
        error: null
    },
    validations: {
        firstName: {
            required: required
        },
        lastName: {
            required: required
        },
        email: {
            required: required,
            email: email
        },
        course: {
            required: required
        }
    },
    methods: {
        create: async function create() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var data = {
                firstName: self.firstName,
                lastName: self.lastName,
                phone: self.phone,
                email: self.email,
                courseId: self.course,
                australianResident: self.australianResident,
                startDate: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY')
            }
            axios.post('/applications/create/', data).then(function (response) {
                
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

            await self.sleep(7 * 1000);

            window.location.href = "/applications/index/";
        },
        edit: async  function edit() {
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
                phone:self.phone,
                email: self.email,
                courses: self.course,
                australianResident: self.australianResident,
                startDate: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY')
            }
            axios.post('/applications/edit/', data).then(function (response) {
                self.success = true;
                self.loader = false;
                setTimeout(() => { self.success = false; }, 5000);
                window.location.href = "/applications/index/";
            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 3000);
            });
            await self.sleep(7 * 1000);

            window.location.href = "/applications/index/";
        },
        loadDictionaries: function loadDictionaries() {
            var self = this;
            axios.get('/applications/dictionaries/').then(function (response) {
                self.courses = response.data.courses;
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
                self.phone = model.Phone;
                self.course = model.CourseId;
                self.startDate = moment(model.StartDate, "DD/MM/YYYY").toDate();
                self.australianResident = model.AustralianResident;

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
        sleep: function sleep(ms) {
            return new Promise((resolve, reject) => setTimeout(resolve, ms));
        }
    },
    mounted: function mounted() {
        var self = this;
        self.loadDictionaries();
        self.initForm(window.model);
        $("body").keyup(function (event) {
            if (event.keyCode === 13) {
                self.create();
            }
        });
    }
});