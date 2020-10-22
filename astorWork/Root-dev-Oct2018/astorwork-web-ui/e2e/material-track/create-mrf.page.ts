import { element, browser, by, Key } from 'protractor';

export class CreateMRFPage {

    getPage() {
        return browser.get(`/material-tracking/create-mrf`);
    }

    fillForm(blkIndex: number, level: string, zone: string, vendorIndex: number) {
        var EC = browser.ExpectedConditions;
        // Select block
        var blkSelect = element(by.css('mat-select[placeholder="Block"]'));
        browser.wait(EC.visibilityOf(blkSelect), 500, "Block select not visible").then(() => {
            var activeOptions = element.all(by.className('option-blk'));
            browser.actions().mouseMove(blkSelect).click().perform().then(function () {
                browser.wait(EC.visibilityOf(activeOptions.get(0)), 1000, 'Block option is not visible').then(() => {
                    browser.actions().mouseMove(activeOptions.get(blkIndex)).click().perform();
                })
            });
        });
        // Key in level
        var levelInput = element(by.css('input[placeholder="Level"]'));
        browser.wait(EC.visibilityOf(levelInput), 500, "Level input not visible").then(() => {
            browser.actions().mouseMove(levelInput).click().sendKeys(level).perform();
        });


        // Key in zone
        var zoneInput = element(by.css('input[placeholder="Zone"]'));
        browser.wait(EC.visibilityOf(zoneInput), 500, "Zone input not visible").then(() => {
            browser.actions().mouseMove(zoneInput).click().sendKeys(zone).perform();
        });

        // Key in casting date (3 days from today)
        var castingDateInput = element(by.css('input[placeholder="Slab casting date"]'));
        browser.wait(EC.visibilityOf(castingDateInput), 500, "Casting date input not visible").then(() => {
            var date = new Date();
            date.setDate(date.getDate() + 3);
            browser.actions().mouseMove(castingDateInput).click().sendKeys(date.toLocaleDateString('en-US')).perform();
        });
        // Select material types
        var materialTypeSelect = element(by.css('mat-select[placeholder="Component Type(s)"]'));
        browser.wait(EC.visibilityOf(materialTypeSelect), 500, "Material type select not visible").then(() => {
            var activeOptions = element.all(by.className('option-material-type'));
            browser.actions().mouseMove(materialTypeSelect).click().perform().then(function () {
                browser.wait(EC.visibilityOf(activeOptions.get(0)), 1000, 'Material type option is not visible').then(() => {
                    activeOptions.count().then((materialTypeCount: number) => {
                        var selectCount = Math.random() * materialTypeCount;
                        for (var i = 0; i <= selectCount; i++) {
                            browser.actions().mouseMove(activeOptions.get(i)).click().perform();
                        }
                        // Need to "ESC" to hide material type selection
                        browser.actions().sendKeys(Key.ESCAPE).perform();
                    });
                })
            });
        });
        // Select vendor
        var vendorSelect = element(by.css('mat-select[placeholder="Vendor"]'));
        browser.wait(EC.visibilityOf(vendorSelect), 500, "Vendor select not visible").then(() => {
            var activeOptions = element.all(by.className('option-vendor'));
            browser.actions().mouseMove(vendorSelect).click().perform().then(function () {
                browser.wait(EC.visibilityOf(activeOptions.get(0)), 1000, 'Vendor option is not visible').then(() => {
                    browser.actions().mouseMove(activeOptions.get(vendorIndex)).click().perform();
                })
            });
        });
        // Select attention person(s)
        var attentionSelect = element(by.css('mat-select[placeholder="Attention"]'));
        browser.wait(EC.visibilityOf(attentionSelect), 500, "Attention select not visible").then(() => {
            var activeOptions = element.all(by.className('option-officer'));
            browser.actions().mouseMove(attentionSelect).click().perform().then(function () {
                browser.wait(EC.visibilityOf(activeOptions.get(0)), 1000, 'Attention option is not visible').then(() => {
                    activeOptions.count().then((attentionCount: number) => {
                        var selectCount = Math.floor(Math.random() * attentionCount);
                        for (var i = 0; i <= selectCount; i++) {
                            browser.actions().mouseMove(activeOptions.get(i)).click().perform();
                        }
                        // Need to "ESC" to hide attention selection
                        browser.actions().sendKeys(Key.ESCAPE).perform();
                    });
                })
            });
        });
        // Delivery date will be calculated
        var deliveryDateInput = element(by.css('input[placeholder="Estimated delivery date"]'));
    }

    submitForm() {
        var EC = browser.ExpectedConditions;
        var submitButton = element(by.css('button[type=submit]'));
        var resultDlg = element(by.tagName('app-create-mrf-result-dlg'));

        browser.wait(EC.visibilityOf(submitButton), 2000, 'Submit button is not visible').then(function () {
            browser.actions().mouseMove(submitButton).click().perform().then(() => {
                browser.sleep(2000);
            });
        })

        return resultDlg.isPresent();
    }

    getMRFNo() {
        var EC = browser.ExpectedConditions;
        var mrfNo = element(by.className('mrf-number'));
        browser.wait(EC.visibilityOf(mrfNo), 1000, 'MRF No. is not visible');
        return mrfNo.getText();
    }

    getMaterialCount() {
        var EC = browser.ExpectedConditions;
        var materialCount = element(by.className('material-count'));
        browser.wait(EC.visibilityOf(materialCount), 1000, 'Material count is not visible');
        return materialCount.getText();
    }
}