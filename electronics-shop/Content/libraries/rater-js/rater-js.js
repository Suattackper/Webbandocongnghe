//const allStar = document.querySelectorAll('.rating .star');
//const ratingValue = document.querySelector('.rating input');

//allStar.forEach((item, idx) => {
//	item.addEventListener('click', function () {
//		let click = 0;
//		ratingValue.value = idx + 1;

//		allStar.forEach(i => {
//			i.classList.replace('bi-star-fill', 'bi-star')
//			i.classList.remove('active')
//		});

//		for (let i = 0; i < allStar.length; i++) {
//			if (i <= idx) {
//				allStar[i].classList.replace('bi-star', 'bi-star-fill')
//				allStar[i].classList.add('active')
//			} else {
//				allStar[i].style.setProperty('--i', click)
//				click++
//			}
//		}
//	});
//});

const allStar = document.querySelectorAll('.rating .star');
const ratingValue = document.querySelector('.rating input');
// Bi?n ?? theo dõi vi?c ch?n sao
let isStarSelected = false;

allStar.forEach((item, idx) => {
	item.addEventListener('click', function () {
		let click = 0;
		ratingValue.value = idx + 1;

		allStar.forEach(i => {
			i.classList.replace('bi-star-fill', 'bi-star')
			i.classList.remove('active')
		});

		for (let i = 0; i < allStar.length; i++) {
			if (i <= idx) {
				allStar[i].classList.replace('bi-star', 'bi-star-fill')
				allStar[i].classList.add('active')
			} else {
				allStar[i].style.setProperty('--i', click)
				click++
			}
		}
		// Ki?m tra xem có ít nh?t m?t sao ???c ch?n sau m?i l?n click
		isStarSelected = true;
	});
});



// X? lý khi ng??i dùng g?i x?p h?ng
// Ví d?: S? d?ng m?t nút ho?c form
submitButton.addEventListener('click', function (event) {
	event.preventDefault(); // Ng?n ch?n hành ??ng m?c ??nh c?a nút ho?c form

	if (!isStarSelected) {
		// N?u không có sao nào ???c ch?n, thông báo cho ng??i dùng và ng?n ng??i dùng g?i x?p h?ng
		alert('Please choose at least one star!');
		return; // Ng?n ch?n vi?c g?i x?p h?ng n?u không có sao nào ???c ch?n
	}

	// N?u có ít nh?t m?t sao ???c ch?n, ti?n hành g?i x?p h?ng
	// Th?c hi?n hành ??ng g?i x?p h?ng ? ?ây
	console.log('?ã g?i x?p h?ng!');
});