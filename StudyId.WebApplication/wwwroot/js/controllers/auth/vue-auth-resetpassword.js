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
    el: '#reset-app',
    data: {
        password: null,
        newpassword: null,
        token: null,
        loader: false,
        success:false,
        error:null
    },
    validations: {
        password: {
            required: required
        },
        newpassword: {
            required: required,
            pwdmatch:pwdmatch
        }
    },
    methods: {
        resetpassword: function resetpassword() {
            var self = this;
            self.$v.$touch();
            if (self.$v.$invalid) {
                return;
            }
            var data = {
                newpassword: self.newpassword,
                token: self.token
            }
            axios.post('/auth/reset/'+self.token, data).then(function (response) {
                self.loader = false;
                self.success = true;
            }).catch(function (error) {
                self.error = error.response.data;
                self.loader = false;
                setTimeout(() => { self.error = null; }, 3000);
            });
        }
    },
    mounted: function mounted() {
        var self = this;
        self.token= window.token;
    }
});