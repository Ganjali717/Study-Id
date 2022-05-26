"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language;
var app = new Vue({
    el: '#login-app',
    data: {
        returnUrl:null,
        username: null,
        password: null,
        loader: false,
        error: null
    },
    validations: {
        username: {
            required: required,
            email:email
        },
        password: {
            required: required
        }
    },
    methods: {
        signIn: function signIn() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                return;
            }
            self.loader = true;
            var data = {
                returnUrl: self.returnUrl,
                username: self.username,
                password: self.password
            }
            axios.post('/auth/login/', data).then(function (response) {
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
        self.returnUrl = window.returnUrl;
        $("body").keyup(function(event) {
            if (event.keyCode === 13) {
                self.signIn();
            }
        });
    }
});