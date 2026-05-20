# Sprint Retrospective Summary — Sprint 8

## Šta je išlo dobro
- Tim je uspješno isporučio sve četiri planirane stavke sprinta (PBI-022, PBI-023, PBI-024, PBI-025), uključujući kompleksnu logiku generisanja rute i ručnog preuređivanja
- Implementacija algoritma za optimizaciju rute i preračunavanje vremena dolaska prošla je bez većih prepreka zahvaljujući dobro definisanim acceptance kriterijima
- Decision Log i AI Usage Log su redovno ažurirani tokom sprinta, što je osiguralo transparentnost svih ključnih tehničkih odluka
- Testna pokrivenost je značajno povećana — napisano je 26 automatskih testova (BLL + DAL) koji pokrivaju sve kritične scenarije

## Šta nije išlo dobro
- Integracijsko testiranje kontrolera (Api.Tests projekt) je ostalo zaostalo iz prethodnih sprintova i nije riješeno u ovom sprintu, što je ograničilo pokrivenost API sloja
- Frontend testiranje Leaflet mapa nije moguće u jsdom okruženju, pa vizualizacija rute ostaje bez automatskih testova

## Šta treba promijeniti
- Posvetiti dio narednog sprinta uspostavljanju Api.Tests projekta kako bi kontroleri bili pokriveni testovima ravnomjerno sa BLL i DAL slojevima
- Istražiti mogućnost integracionog/e2e testiranja frontenda (npr. Playwright) za komponente koje zavise od DOM API-ja koji jsdom ne podržava

## Konkretne akcije koje tim uvodi u Sprint 9
- Postaviti jasnu granicu između unit i integracijskih testova i osigurati da oba tipa budu prisutna za svaki novi PBI koji se implementira
- Planirati sprintove sa funkcionalnostima vezanim za poštara (mobilni prikaz rute, ažuriranje statusa sandučića) koje donose novi nivo tehničke složenosti i korisnički vidljivu vrijednost
