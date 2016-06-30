// Fix Electron not seeing JQuery
// http://stackoverflow.com/questions/32621988/electron-jquery-is-not-defined
if (typeof module === 'object') { window.module = module; module = undefined; }
