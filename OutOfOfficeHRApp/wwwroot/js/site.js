// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function removePhoto(id) {
    const token = getCsrfToken()

    $.ajax({
        url: `${id}/Remove`,
        type: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestToken': token,
        },
        success: function (data) {
            alert(data);
            window.location.reload();
        },
        error: function (error) {
            console.error('Error: ', error);
            alert('Was error when trying delete photo');
        }
    })
}

const getCsrfToken = () => {
    const tokenCookie = document.cookie.split('; ').find(row => row.startsWith('XSRF='));
    return tokenCookie ? tokenCookie.split('=')[1] : '';
}

//const sortTable = () => {
//    $('th').on('click', function () {
//        var column = $(this).data('column');
//        var index = $(this).index();
//        var order = $(this).data('order')
//        console.log(column, index, order)

//        if (order == 'asc') {
//            $(this).data('order', 'desc').text($(this).text() + ' ▲')
//            "▼▲"
//        }
//        else if (order == 'desc') {
//            $(this).data('order', '').text($(this).text().replace(' ▲', ' ▼'))
//        }
//        else {
//            $(this).data('order', 'asc').text($(this).text().replace(' ▼', ''))
//        }

//        var tbody = $('tbody')
//        var rows = $(tbody).find('tr').toArray();

//        rows.sort(function (rowA, rowB) {
//            var valueA = $(rowA).find('td').eq(index).text().trim();
//            var valueB = $(rowB).find('td').eq(index).text().trim();

//            if ($.isNumeric(valueA) && $.isNumeric(valueB)) {
//                valueA = parseFloat(valueA);
//                valueB = parseFloat(valueB);
//            }
//            if (order == 'asc') {
//                return valueA > valueB ? 1 : valueA < valueB ? -1 : 0
//            }
//            else if (order == 'desc') {
//                return valueA < valueB ? 1 : valueA > valueB ? -1 : 0
//            }

//        })

//        if (order !== 'asc' && order !== 'desc') {
//            rows.sort(function (rowA, rowB) {
//                var valueA = $(rowA).find('td').eq(0).text().trim();
//                var valueB = $(rowB).find('td').eq(0).text().trim();
//                return parseFloat(valueA) - parseFloat(valueB);
//            });
//        }
//        tbody.empty().append(rows);
//    })
//}

const sortTable = () => {
    const headers = document.querySelectorAll('th');

    headers.forEach(header => {
        header.addEventListener('click', () => {
            const column = header.getAttribute('data-column');
            const index = Array.prototype.indexOf.call(headers, header);
            let order = header.getAttribute('data-order');
            console.log(column, index, order)

            headers.forEach(h => {
                h.setAttribute('data-order', '');
                h.innerHTML = h.innerHTML.replace(' ▲', '').replace(' ▼', '');
            });

            if (order === 'asc') {
                order = 'desc'
                header.innerHTML = header.textContent + ' ▼';
            }
            else if (order === 'desc') {
                order = '';
                header.innerHTML = header.textContent;
            }
            else {
                order = 'asc';
                header.innerHTML = header.textContent + ' ▲';
            }
            header.setAttribute('data-order', order);

            const tbody = document.querySelector('tbody');
            const rows = Array.from(tbody.querySelectorAll('tr'));

            if (order !== '') {
                rows.sort((rowA, rowB) => {
                    let valueA = rowA.cells[index].textContent.trim();
                    let valueB = rowB.cells[index].textContent.trim();

                    if (!isNaN(valueA) && !isNaN(valueB)) {
                        valueA = parseFloat(valueA);
                        valueB = parseFloat(valueB);
                    }

                    if (order === 'asc') {
                        return valueA > valueB ? 1 : valueA < valueB ? -1 : 0;
                    } else {
                        return valueA < valueB ? 1 : valueA > valueB ? -1 : 0;
                    }
                });
            }
            else {
                rows.sort((rowA, rowB) => {
                    const defaultA = parseFloat(rowA.cells[0].textContent.trim());
                    const defaultB = parseFloat(rowB.cells[0].textContent.trim());
                    return defaultA > defaultB ? 1 : 0;
                })
            }

            tbody.innerHTML = '';
            rows.forEach(row => {
                tbody.appendChild(row)
            })
        })
    })
}
document.addEventListener('DOMContentLoaded', function () {
    sortTable();
})