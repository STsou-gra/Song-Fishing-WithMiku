mergeInto(LibraryManager.library, {
  PlaySongJS: function () {
    if (window.player) {
      window.player.requestPlay();
    }
  },
  PauseSongJS: function () {
    if (window.player) {
      window.player.requestPause();
    }
  },
});
