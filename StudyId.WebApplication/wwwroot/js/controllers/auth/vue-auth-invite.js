"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language,
    pwdmatch = (value, vm) => (vm.password == null || value==null || vm.password==='' || value==='') ||  (vm.password.length > 0 && value === vm.password);
var app = new Vue({
    el: '#invite-app',
    data: {
        token:null,
        password: null,
        confirmPassword: null,
        firstName: null,
        lastName: null,
        loader: false,
        error: null
    },
    validations: {
        password: {
            required: required
        },
        confirmPassword: {
            required: required,
            pwdmatch:pwdmatch
        },
        firstName: {
            required: required
        },
        lastName: {
            required: required
        }
    },
    methods: {
        register: function register() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var data = {
                password: self.password,
                firstname:self.firstName,
                lastname:self.lastName
            }
            axios.post('/auth/invite/'+self.token, data).then(function (response) {
                if (response.data.success === true) {
                    window.location.href = response.data.message;
                }
                else {
                    self.error = response.data.message;
                }
            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 3000);
            });
        }
    },
    mounted: function mounted() {
        var self = this;
        self.token = window.model.Password;
        self.firstName = window.model.FirstName;
        self.lastName = window.model.LastName;
    }
});