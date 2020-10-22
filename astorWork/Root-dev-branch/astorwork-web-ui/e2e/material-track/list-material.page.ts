import { element, browser, by, Key } from 'protractor';

export class ListMaterialPage {
    getPage() {
        return browser.get(`/material-tracking/materials`);
    }

    getPageTitle() {
        return browser.getTitle();
    }

    getPageHeader() {
        var EC = browser.ExpectedConditions;
        var header = element(by.tagName('h1'));
        browser.wait(EC.visibilityOf(header), 5000, 'Page header is not visible');
        return header.getText();
    }

    getMaterialsCount(blkIndex: number) {
        var EC = browser.ExpectedConditions;
        var blkSelect = element(by.id('blkSelect'));
        var blkOption = element.all(by.css('.ng-trigger .mat-option')).get(blkIndex);
        var materialRange = element.all(by.className('mat-paginator-range-label'));

        browser.actions().mouseMove(blkSelect).click().perform().then(function () {
            browser.wait(EC.visibilityOf(blkOption), 1000, 'Block option is not visible');

            browser.actions().mouseMove(blkOption).click().perform();
            browser.sleep(3000);
        });

        return materialRange.getText();
    }

    getMaterialsCountAfterFilter(filterValue: string) {
        var filterField = element(by.css('input[placeholder="Filter"]'));

        var materialRange = element.all(by.className('mat-paginator-range-label'));
        filterField.clear().then(function () {
            browser.actions().mouseMove(filterField).click().sendKeys(filterValue).perform().then(function () {
                browser.sleep(1000);
            });
        })
        return materialRange.getText();
    }

    navigateToMaterialDetails(rowIndex: number) {
        var EC = browser.ExpectedConditions;
        var filterField = element(by.css('input[placeholder="Filter"]'));
        var materialRow = element.all(by.tagName('mat-row')).get(rowIndex);
        filterField.clear().then(() => {
            // Key in dummy filter then clear, trigger the on filter event
            browser.actions()
                .sendKeys('1')
                .sendKeys(Key.BACK_SPACE)
                .perform().then(function () {
                    browser.sleep(1000);
                    materialRow.click();
                    browser.wait(EC.urlContains('/materials/'), 5000, 'Redirect to material details failed');
                })
        });

        return browser.getCurrentUrl();
    }
}