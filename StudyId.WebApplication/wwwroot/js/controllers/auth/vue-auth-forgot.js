"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language;
var app = new Vue({
    el: '#forgot-app',
    data: {
        username: null,
        loader: false,
        error:null,
        success:false
    },
    validations: {
        username: {
            required: required,
            email:email
        }
    },
    methods: {
        forgotPassword: function forgotPassword() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var data = {
                email: self.username
            }
            axios.post('/auth/forgot/', data).then(function (response) {
                self.success = true;
                self.loader = false;
                setTimeout(() => { self.success = false; }, 5000);
            }).catch(function (error) {
                self.loader = false;
                self.error = error.response.data;
                setTimeout(() => { self.error = null; }, 3000);
            });
        }
    },
    mounted: function mounted() {
        var self = this;
    }
});