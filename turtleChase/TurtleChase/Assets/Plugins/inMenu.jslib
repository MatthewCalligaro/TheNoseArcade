mergeInto(LibraryManager.library, {

  updateMenuStatus: function (isInMenu) {
    console.log("library function says menu status is "+Boolean(isInMenu));
    document.getElementById("menuStatus").setAttribute("menu-status", Boolean(isInMenu));
  },

});