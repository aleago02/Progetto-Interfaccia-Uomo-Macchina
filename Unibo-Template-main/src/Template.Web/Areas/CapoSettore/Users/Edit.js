var CapoSettore;
(function (CapoSettore) {
    var Users;
    (function (Users) {
        class editVueModel {
            constructor(hub, model) {
                this.hub = hub;
                this.model = model;
                if (this.hub) {
                    this.hub.on("NewMessage", async (idUser, idMessage) => {
                        // do stuff with parameters
                    });
                }
            }
        }
        Users.editVueModel = editVueModel;
    })(Users = CapoSettore.Users || (CapoSettore.Users = {}));
})(CapoSettore || (CapoSettore = {}));
//# sourceMappingURL=Edit.js.map