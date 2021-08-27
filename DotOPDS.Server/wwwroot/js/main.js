
var mimeTypes = {
  'application/fb2': 'fb2',
  'application/fb2+zip': 'fb2.zip',
  'application/epub+zip': 'epub',
  'application/x-mobipocket-ebook': 'mobi',
  'application/vnd.amazon.ebook': 'azw',
  'image/x-djvu': 'djv',
  'application/msword': 'doc',
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document': 'docx',
  'application/pdf': 'pdf',
  'text/plain': 'txt',
  'text/rtf': 'rtf',
}

var entryTemplate

Handlebars.registerHelper('ifCond', function (v1, operator, v2, options) {
  switch (operator) {
    case '==':
      return (v1 == v2) ? options.fn(this) : options.inverse(this)
    case '===':
      return (v1 === v2) ? options.fn(this) : options.inverse(this)
    case '<':
      return (v1 < v2) ? options.fn(this) : options.inverse(this)
    case '<=':
      return (v1 <= v2) ? options.fn(this) : options.inverse(this)
    case '>':
      return (v1 > v2) ? options.fn(this) : options.inverse(this)
    case '>=':
      return (v1 >= v2) ? options.fn(this) : options.inverse(this)
    case '&&':
      return (v1 && v2) ? options.fn(this) : options.inverse(this)
    case '||':
      return (v1 || v2) ? options.fn(this) : options.inverse(this)
    default:
      return options.inverse(this)
  }
})

function isGoodBroswer() {
  var result = false
  var MyBlob = NewBlob('test text', 'text/plain');
  if (MyBlob instanceof Blob) {
    var url = createObjectURL(MyBlob)
    if (url != null) {
      result = true
      URL.revokeObjectURL(url)
    }
  }
  return result
}

function createObjectURL(file) {
  if (window.URL && window.URL.createObjectURL) {
    return window.URL.createObjectURL(file);
  } else if (window.webkitURL) {
    return window.webkitURL.createObjectURL(file);
  } else {
    return null;
  }
}

function revokeObjectURL(file) {
  if (window.URL && window.URL.revokeObjectURL) {
    return window.URL.revokeObjectURL(file);
  } else if (window.webkitURL) {
    return window.webkitURL.revokeObjectURL(file);
  } else {
    return null;
  }
}

function NewBlob(data, datatype) {
  var out = null;

  try {
    out = new Blob([data], {type: datatype});
  } catch (e) {
    window.BlobBuilder = window.BlobBuilder ||
        window.WebKitBlobBuilder ||
        window.MozBlobBuilder ||
        window.MSBlobBuilder;

    if (e.name == 'TypeError' && window.BlobBuilder) {
      var bb = new BlobBuilder();
      bb.append(data);
      out = bb.getBlob(datatype);
    }
    else if (e.name == "InvalidStateError") {
      // InvalidStateError (tested on FF13 WinXP)
      out = new Blob([data], {type: datatype});
    }
    else {
      // nope
    }
  }
  return out;
}

function saveFile(data, fileName) {
  var a = document.createElement("a");
  document.body.appendChild(a);
  a.style = "display: none";
  var url = window.URL.createObjectURL(data);
  a.href = url;
  a.download = fileName;
  a.click();
  setTimeout(function(){
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }, 100)
}

function getParameterByName(name, url) {
  if (!url) url = window.location.href;
  url = url.toLowerCase(); // This is just to avoid case sensitiveness
  name = name.replace(/[\[\]]/g, "\\$&").toLowerCase();// This is just to avoid case sensitiveness for query parameter name
  var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
      results = regex.exec(url);
  if (!results) return null;
  if (!results[2]) return '';
  return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function UpdateQueryString(key, value, url) {
  if (!url) url = window.location.href;
  var re = new RegExp("([?&])" + key + "=.*?(&|#|$)(.*)", "gi"),
    hash;

  if (re.test(url)) {
    if (typeof value !== 'undefined' && value !== null)
      return url.replace(re, '$1' + key + "=" + value + '$2$3');
    else {
      hash = url.split('#');
      url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
      if (typeof hash[1] !== 'undefined' && hash[1] !== null)
        url += '#' + hash[1];
      return url;
    }
  }
  else {
    if (typeof value !== 'undefined' && value !== null) {
      var separator = url.indexOf('?') !== -1 ? '&' : '?';
      hash = url.split('#');
      url = hash[0] + separator + key + '=' + value;
      if (typeof hash[1] !== 'undefined' && hash[1] !== null)
        url += '#' + hash[1];
      return url;
    }
    else
      return url;
  }
}

function SearchQuery(page) {
  return ' data-page="' + page + '" href="' + UpdateQueryString('page', page == 1 ? null : page) + '"'
}

function paginate(page, total, max) {
  var sp = 1
  var res = ''

  if (total < max) max = total
  // Starting point
  if (page < max) {
    sp = 1
  } else if (page >= (total - Math.floor(max / 2))) {
    sp = total - max + 1
  } else if (page >= max) {
    sp = page - Math.floor(max / 2)
  }

  if (total > 1) {
    res += "<ul class=\"pagination\">"
    res += "<li"
    if (page <= 1) res += " class=\"disabled\""
    res += ">"
    res += "<a aria-label=\"Previous\""
    if (page > 1) res += SearchQuery(page - 1)
    res += ">"

    res += "<span aria-hidden=\"true\">&laquo;</span>"
    res += "</a>"
    res += "</li>"

    for (var i = sp; i <= (sp + max - 1); i++) {
      res += "<li"
      if (page == i) res += " class=\"disabled\""
      res += ">"
      res += "<a"
      if (page != i) res += SearchQuery(i)
      res += ">" + i + "</a>"
      res += "</li>"
    }

    res += "<li"
    if (page + 1 > total) res += " class=\"disabled\""
    res += ">"
    res += "<a aria-label=\"Next\""
    if (page < total) res += SearchQuery(page + 1)
    res += ">"
    res += "<span aria-hidden=\"true\">&raquo;</span>"
    res += "</a>"
    res += "</li>"
    res += "</ul>"
  }

  return res
}

History.Adapter.bind(window, 'statechange', function () {
  doSearch()
})

function updateUrl(query, page) {
  query = query || decodeURIComponent(document.location.pathname.substr(1))
  var url = '/' + encodeURIComponent(query)
  if (page > 1) url += '?page=' + page
  History.pushState(null, document.title, url)
}

function appReady() {
  var entrySource = $("#entry-template").html()
  entryTemplate = Handlebars.compile(entrySource)

  NProgress.start()
  $.getJSON('/opds')
    .done(function (data) {
      document.title = data.title
      $('h3.title').text(data.title)
    }).always(function () {
      NProgress.done()
    })

  $('form').submit(searchSubmit)
  $('nav').on('click', 'ul > li > a', pageClick)
  if(isGoodBroswer()) $(document).on('click', 'a[role="download"]', downloadClick)

  var query = document.location.pathname.substr(1)
  $('input[name=q]').val(decodeURIComponent(query))
  doSearch()
}

function pageClick(e) {
  e.preventDefault()
  var page = $(this).data('page')
  if (page) {
    updateUrl(null, page)
  }
}

function downloadClick(e) {
  e.preventDefault()
  var $el = $(this)
  var link = $el.attr('href')
  var idx = $el.data('index')
  var $progress = $('a[data-main="' + idx + '"]')

  $progress
    .children('.btn-checkmark')
    .removeClass('active')
  $progress
    .children('.btn-cross')
    .removeClass('active')
  $progress
    .children('.btn-spinner')
    .addClass('active')

  $.ajax({
    url: link,
    type: 'GET',
    dataType: 'binary',
    processData: false
  })
  .success(function (data, textStatus, jqXHR) {
    try {
      var header = jqXHR.getResponseHeader('Content-Disposition')
      var disposition = mimecodec.parseHeaderValue(header)
      var name = mimecodec.mimeWordsDecode(disposition.params.filename)
      saveFile(data, name)
      $progress
        .children('.btn-checkmark')
        .addClass('active')
    } catch (err) {
      $progress
        .children('.btn-spinner')
        .removeClass('active')
    }
  })
  .fail(function () {
    $progress
      .children('.btn-cross')
      .addClass('active')
  })
  .always(function () {
    $progress
      .children('.btn-spinner')
      .removeClass('active')
  })
}

function searchDone(data) {
  $entries = $('#entries')
  $entries.empty()
  document.title = data.title
  if (data.total <= 0) {
    // not found
    $('#search-info-not-found').show()
    $('#search-info-found').hide()
    $('.pagination-fix').hide()
    return
  }

  $('#wrapper').removeClass('vertical-center')
  $('#search-info-total').text(data.total)
  $('#search-info-found').show()
  $('#search-info-not-found').hide()

  var i = 0
  data.entries.forEach(function (book) {
    book.idx = i++
    book.downloads = []
    book.links.forEach(function (link) {
      if (link.href.indexOf('download/file/') > 0) {
        var format = mimeTypes[link.type]
        book.downloads.push({format: format, href: link.href})
      }
      if (link.rel == 'http://opds-spec.org/image') {
        book.cover = link.href
      }
    })
    book.downloadMain = book.downloads[0]
    book.downloads.shift()
    var el = entryTemplate(book)
    $entries.append(el)
  })

  var totalPages = Math.ceil(data.total / data.itemsPerPage)
  if (totalPages > 1) {
    var currentPage = +getParameterByName('page') || 1
    var paginator = paginate(currentPage, totalPages, 5)
    $('.pagination-fix').html(paginator).show()
  } else {
    $('.pagination-fix').hide()
  }

  window.scrollTo(0, 0)
}

function searchSubmit(e) {
  e.preventDefault()
  var query = $('input[name=q]').val().trim()
  updateUrl(query)
}

function doSearch() {
  var query = document.location.pathname.substr(1)
  if (!query) return
  page = +getParameterByName('page') || 1
  var q = decodeURIComponent(query)

  NProgress.start()
    $.getJSON('/opds/search/everywhere', {q: q, page: page})
    .done(searchDone)
    .always(NProgress.done)
}

$(appReady)
