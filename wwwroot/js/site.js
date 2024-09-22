window.selectTextarea = (element) => {
  if (element) {
      element.focus();
      element.select();
  }
};

function scrollToBottom(element) {
  element.scrollTop = element.scrollHeight;
}


window.getScrollInfo = function (element) {
  return {
      scrollHeight: element.scrollHeight,
      scrollTop: element.scrollTop,
      clientHeight: element.clientHeight
  };
};