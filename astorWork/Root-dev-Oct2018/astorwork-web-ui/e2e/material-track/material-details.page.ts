import { element, browser, by, Key } from 'protractor';

export class MaterialDetailPage {
    getPage(id: number) {
        return browser.get('material-tracking/materials/' + id);
    }

    getPageTitle() {
        return browser.getTitle();
    }

    getTableTitle() {
        return element(by.className('mat-h1')).getText();
    }

    getMarkingNo() {
        return element(by.css('input[placeholder="Marking No."]')).getAttribute('value');
    }

    getStageName(stageNum: number) {
        return element.all(by.className('stage-name')).get(stageNum).getText();
    }

    getStageIcon(stageNum: number) {
        return element.all(by.className('mat-list-icon')).get(stageNum).element(by.tagName('mat-icon')).getText();
    }
}