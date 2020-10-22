import { element, browser, by, Key } from 'protractor';

export class ListMRFPage {
    getPage() {
        return browser.get(`/material-tracking/mrfs`);
    }

    getPageHeader() {
        var EC = browser.ExpectedConditions;
        var header = element(by.tagName('h1'));
        browser.wait(EC.visibilityOf(header), 5000, 'Page header is not visible');
        return header.getText();
    }

    selectBlk(blkIndex: number) {
        var EC = browser.ExpectedConditions;
        var blkSelect = element(by.id('blkSelect'));
        var blkOption = element.all(by.css('.ng-trigger .mat-option')).get(blkIndex);

        browser.actions().mouseMove(blkSelect).click().perform().then(function () {
            browser.wait(EC.visibilityOf(blkOption), 1000, 'Block option is not visible').then(() => {
                browser.actions().mouseMove(blkOption).click().perform();
            })
        });
    }

    getMRFsCount() {
        var mrfRange = element(by.className('mat-paginator-range-label'));

        return mrfRange.getText();
    }

}