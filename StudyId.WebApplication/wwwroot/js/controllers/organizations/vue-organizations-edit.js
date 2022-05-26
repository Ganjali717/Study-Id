"use strict";
Vue.use(window.vuelidate.default);
var _window$validators = window.validators,
    required = _window$validators.required,
    helpers = _window$validators.helpers,
    email = _window$validators.email,
    minLength = _window$validators.minLength,
    _window$validators$la = _window$validators.language
Vue.component('v-select', VueSelect.VueSelect);
var app = new Vue({
    el: '#organization-edit',
    data: {
        courses: [],
        statuses: [],
        orgCourses: [],
        orgPeople: [],
        status: [],
        document: [],

        id: null,
        successId: null,

        title: null,
        taxNumber: null,
        address: null,
        phone: null,
        organizationEmail: null,

        startDate: null,
        person: null,
        created: null,
        course: null,
        success: false,
        loader: false,
        error: null,
        show: true,
        visible: null,
        selectedCourses: []
    },
    validations: {
        title: { required: required },
        taxNumber: { required: required },
        address: { required: required },
        phone: { required: required },
        startDate: { required: required },
        organizationEmail: {
            required: required,
            email: email
        },
        orgCourses: {
            required: required,
            $each: {
                CourseId: {
                    required
                },
                LocalName: {
                    required
                },
                Duration: {
                    required
                },
                Price: {
                    required
                }
            }
        },
        orgPeople: {
            required: required,
            $each: {
                FirstName: {
                    required
                },
                LastName: {
                    required
                },
                Email: {
                    required,
                    email
                },
                Phone: {
                    required
                },
                Position: {
                    required
                }
            }
        },
        document: {
            required: required
        },
        status: {
            required: required
        }

    },
    methods: {
        vadded: function vadded(e) {
            var files = e.target.files || e.dataTransfer.files;
            document.getElementById("downloadedfiles").style.display = 'block';
            var formData = new FormData();

            if (files.size > 200000000) {
                e.preventDefault();
                alert('Files too big (> 20MB)');
                return;
            }

            for(var i=0; i < files.length; i++)
            {
                var fsize = files[i].size;
                var file = Math.round((fsize / 1024));
                if (file >= 5242880) {
                    e.preventDefault();
                    alert(e.target.files[i].name + 'file too big (> 5MB)');
                    return;
                }
                formData.append('file', files[i]);
                var obj = {};
                obj["name"] = e.target.files[i].name;
                this.document.push(obj);
                e.target.getAttribute("data-index") === e.target.files[i].name;
            }
            axios({
                method: "post",
                url: "/organizations/upload",
                data: formData,
                headers: { "Content-Type": "text/plain" }
            }).then(function (response) {
            }).catch(function (response) {
            });
        },
        downloadFile: function downloadFile(e) {
            var fileName = e.target.innerHTML;
            var self = this;
            axios.get('/organizations/download/' + fileName).then(function (response) {
                var fileURL = window.URL.createObjectURL(new Blob([response.data]));
                var fileLink = document.createElement('a');

                fileLink.href = fileURL;
                fileLink.setAttribute('download', fileName);
                document.body.appendChild(fileLink);

                fileLink.click();
            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        deleteFile: function deleteFile(e) {
            var fileName = e.target.innerHTML;
            var file = fileName.trim();
            var self = this;
            axios.get('/organizations/delete/' + file).then(function (response) {
               
            }).catch(function (error) {
                self.error = error.response.data;
            });
            for (var i=0; i < self.document.length; i++) {
                if (self.document[i].name === file) {
                    var index = self.document.indexOf(self.document[i]);
                    self.document.splice(index,1);
                }
            }
             if (self.document.length === 0) {
                 document.getElementById("downloadedfiles").style.display = 'none';
             }

        },
        create: function create() {
            var self = this;
            this.$v.$touch();
            if (this.$v.$invalid) {
                setTimeout(function () {
                    if ($("input.is-invalid").length > 0) {
                        var errorElm = $("input.is-invalid")[0];
                        $('html, body').animate({
                            scrollTop: ($(errorElm).offset().top-50)
                        }, 200);
                        errorElm.focus();
                    }
                }, 100);
                return;
            }
            self.loader = true;
            var model = {
                title: self.title,
                taxNumber: self.taxNumber,
                address: self.address,
                email: self.organizationEmail,
                phone: self.phone,
                documents: self.document,
                status: self.status,
                persons: self.orgPeople,
                courses: self.orgCourses,
                startDate: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY'),
                created: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY'),
                updated: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY')
            }
            axios.post('/organizations/create', model).then(function (response) {
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
                taxNumber: self.taxNumber,
                address: self.address,
                email: self.organizationEmail,
                phone: self.phone,
                status: self.status,
                persons: self.orgPeople,
                courses: self.orgCourses,
                documents: self.document,
                startDate: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY'),
                created: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY'),
                updated: typeof self.startDate == "undefined" ? null : moment(self.startDate).format('DD/MM/YYYY')
            }
            axios.post('/organizations/edit/', data).then(function (response) {
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
            axios.get('/organizations/dictionaries/').then(function (response) {
                self.courses = response.data.courses;
                self.statuses = response.data.statuses;
            }).catch(function (error) {
                self.error = error.response.data;
            });
        },
        initForm: function initForm(model) {
            var self = this;
            if (model) {
                self.id = model.Id;
                self.title = model.Title;
                self.taxNumber = model.TaxNumber;
                self.address = model.Address;
                self.phone = model.Phone;
                self.organizationEmail = model.Email;
                self.status = model.Status;
                self.orgPeople = model.Persons;
                self.orgCourses = model.Courses;
                self.document = model.Documents;
                self.startDate = moment(model.StartDate, "DD/MM/YYYY").toDate();
            }
        },
        clearForm: function clearForm() {
            var self = this;
            self.course = [];
            self.person = [];
            self.email = null;
            self.startDate = null;
        },
        addCourse: function addCourse() {
            var self = this;
            if (self.loader === true) return;
            let defaultCourse = self.courses[self.courses.length - 1].id;
            self.orgCourses.push({ LocalName: "", CourseId: defaultCourse, Duration: "", Price: "" });
        },
        addPerson: function addPerson() {
            var self = this;
            if (self.loader === true) return;
            let defaultPerson = self.statuses[self.statuses.length - 1].value;
            self.orgPeople.push({ FirstName: "", LastName: "", Email: "", Phone: "", Position: "", Document: "" });
        },
        removeCourse: function removeCourse(item) {
            var self = this;
            if (self.inviteLoader === true) return;
            var index = self.orgCourses.indexOf(item);
            self.orgCourses.splice(index, 1);
        },
        removePerson: function removePerson(item) {
            var self = this;
            if (self.inviteLoader === true) return;
            var index = self.orgPeople.indexOf(item);
            self.orgPeople.splice(index, 1);
        }
    },
    mounted: function mounted() {
        var self = this;
        self.loadDictionaries(); 
        self.initForm(window.model);
    }
});