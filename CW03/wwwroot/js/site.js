// Write your JavaScript code.
$(function () {
    //toggles glyphicon
    $('.list-group-item').on('click', function () {
        $('.glyphicon', this)
            .toggleClass('glyphicon-chevron-right')
            .toggleClass('glyphicon-chevron-down');

        //once a bookmark is clicked, it will show options
        $("#toHide").addClass("hidden");
        $("#toDisplay").removeAttr("class");

        //getting buttons and setting links
        var newBookmark = document.getElementById("newBookMark");
        var BookMarkDetail = document.getElementById("BookMarkDetail");
        var editBookMark = document.getElementById("editBookMark");
        var lockBookMark = document.getElementById("lockBookMark");
        var UnlockBookMark = document.getElementById("UnlockBookMark");
        var deleteBookMark = document.getElementById("deleteBookMark");

        newBookmark.setAttribute("href", "/BookmarkEntities/CreateType?Parent=" + this.getAttribute("href").replace("#", "") + "&returnUrl=Home");
        BookMarkDetail.setAttribute("href", "/BookmarkEntities/DetailsFromPath?Parent=" + this.getAttribute("href").replace("#", "") + "&returnUrl=Home");
        editBookMark.setAttribute("href", "/BookmarkEntities/EditFromPath?Parent=" + this.getAttribute("href").replace("#", "") + "&returnUrl=Home");
        lockBookMark.setAttribute("href", "/BookmarkEntities/LockFromPath?path=" + this.getAttribute("href").replace("#", "") + "&returnUrl=Home");
        UnlockBookMark.setAttribute("href", "/BookmarkEntities/UnlockFromPath?path=" + this.getAttribute("href").replace("#", "") + "&returnUrl=Home");
        deleteBookMark.setAttribute("href", "/BookmarkEntities/DeleteFromPath?Parent=" + this.getAttribute("href").replace("#", "") + "&returnUrl=Home");
    });

});