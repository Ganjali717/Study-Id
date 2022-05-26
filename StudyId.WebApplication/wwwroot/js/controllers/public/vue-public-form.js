"use strict";
var form = document.getElementById('contact-form');
if (form) {
    Vue.use(window.vuelidate.default);
    var _window$validators = window.validators,
        required = _window$validators.required,
        helpers = _window$validators.helpers,
        email = _window$validators.email,
        minLength = _window$validators.minLength,
        _window$validators$la = _window$validators.language;
    Vue.component('v-select', VueSelect.VueSelect);
    var app = new Vue({
        el: '#contact-form',
        data: {
            courses: [],
            id: null,
            successId: null,
            firstName: null,
            lastName: null,
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
            }
        },
        methods: {
            disableBeforeToday: function (date) {
                const today = new Date();
                return date < today;
            },
            submit: function submit() {
                gtag('event', 'try-submit', {'url': window.location.href});
                var self = this;
                this.$v.$touch();
                if (this.$v.$invalid) {
                    return;
                }
                if (self.loader===true) return;
                self.loader = true;
          
                var data = {
                    firstName: self.firstName,
                    lastName: self.lastName,
                    email: self.email,
                    courseId: self.course,
                    australianResident: self.australianResident,
                    startDate: self.startDate
                }
                axios.post('/enroll/', data).then(function (response) {
                    self.success = true;
                    self.loader = false;
                    setTimeout(() => { self.success = false; }, 5000);
                    gtag('event', 'submit-form', { 'url': window.location.href });
                    self.clearForm();
                    self.$v.$reset();
                }).catch(function (error) {
                    self.error = error.response.data;
                    self.loader = false;
                    setTimeout(() => { self.error = null; }, 3000);
                });
            },
            loadDictionaries: function loadDictionaries() {
                var self = this;
                axios.get('/applications/dictionaries/').then(function (response) {
                    self.courses = response.data.courses;
                }).catch(function (error) {
                    self.error = error.response.data;
                });
            },
            clearForm: function clearForm() {
                var self = this;
                self.firstName = null;
                self.lastName = null;
                self.email = null;
                self.course = [];
                self.startDate = null;
                self.australianResident = false;
            }
        },

        mounted: function mounted() {
            var self = this;
            self.loadDictionaries();
            document.addEventListener('keyup', logKey);
            function logKey(e) {
                if (e.keyCode === 13) {
                    self.submit();
                }

            }
        }
    });
}
