import { AppPage } from './app.po';
import { ListMaterialPage } from './material-track/list-material.page';
import { MaterialDetailPage } from './material-track/material-details.page';
import { browser, ElementFinder, Key } from 'protractor';
import { CreateMRFPage } from './material-track/create-mrf.page';
import { ListMRFPage } from './material-track/list-mrf.page';

describe('astorWork Web UI Test Suite', () => {
  const listMaterialPage = new ListMaterialPage();
  const materialDetailPage = new MaterialDetailPage();
  const createMRFPage = new CreateMRFPage();
  const listMRFPage = new ListMRFPage();
  // Navigate to index to get project id
  browser.get('/');
  browser.sleep(2000);

  describe('authentication and session should work fine', () => {
    it('should be redirected to login if session not exist');
    it('should be able to login');
    it('should be able to refresh when page close and reopen');
    it('should be able to refresh when prompted');
    it('should be able to logout when prompted but user cancel');
  });

  describe('list material page should work fine', () => {
    beforeAll(() => {
      listMaterialPage.getPage();
    });

    it('should have some materials under first block', () => {
      listMaterialPage.getMaterialsCount(0).then((countLabel: string) => {
        expect(countLabel).toMatch('[0-9]+$');
      })
    });

    it('should have right title', () => {
      listMaterialPage.getPageHeader()
        .then((title: string) => {
          expect(title).toContain('List of Materials');
        });
    })

    it('should have 2 "K2S50" materials at level 12', () => {
      listMaterialPage.getMaterialsCountAfterFilter('K2S50 12').then((count: string) => {
        expect(count).toMatch('.2');
      })
    });

    it('should have navigate to 1st material after clicking first row', () => {
      listMaterialPage.navigateToMaterialDetails(0).then((url: string) => {
        expect(url).toMatch('.\/materials\/[0-9]+$');
      })
    });
  });

  describe('material detail page for id 1 should work fine', () => {
    it('should have right title', () => {
      materialDetailPage.getTableTitle().then((title: string) => {
        expect(title).toContain('Material Details');
      });
    })

    it('should have right marking no. (K2B50)', () => {
      materialDetailPage.getMarkingNo().then((markingNo: string) => {
        expect(markingNo).toEqual('K2B50');
      })
    })

    it('should have right stage name', () => {
      materialDetailPage.getStageName(5).then((stageName: string) => {
        expect(stageName).toContain('Installed-QC');
      })
    })

    it('should have right stage icon (empty circle)', () => {
      materialDetailPage.getStageIcon(5).then((stageIcon: string) => {
        expect(stageIcon).toContain('location_searching');
      })
    })
  });

  describe('material detail page for id 2 should work fine', () => {
    beforeAll(() => {
      materialDetailPage.getPage(2);
    });

    it('should have right marking no. (K7P50)', () => {
      materialDetailPage.getMarkingNo().then((markingNo: string) => {
        expect(markingNo).toEqual('K7P50');
      })
    })

    it('should have right stage name', () => {
      materialDetailPage.getStageName(2).then((stageName: string) => {
        expect(stageName).toContain('Delivered');
      })
    })

    it('should have right stage icon', () => {
      materialDetailPage.getStageIcon(2).then((stageIcon: string) => {
        expect(stageIcon).toContain('error');
      })
    })
  });

  describe('list mrf page should work fine', () => {
    beforeAll(() => {
      listMRFPage.getPage();
    });

    it('should have 4 records by default', () => {
      listMRFPage.getMRFsCount().then((countLabel: string) => {
        expect(countLabel).toMatch('.4$');
      });
    });

    it('should have 2 records for block 1', () => {
      listMRFPage.selectBlk(1);
      listMRFPage.getMRFsCount().then((countLabel: string) => {
        expect(countLabel).toMatch('.2$');
      });
    });
  });

  return;

  describe('create mrf page should work fine', () => {
    beforeEach(() => {
      createMRFPage.getPage();
    });

    it('should not be able to submit form if form is empty', () => {
      createMRFPage.submitForm().then((resultDialogPresent: boolean) => {
        expect(resultDialogPresent).toBeFalsy();
      });
    });

    it('should not be able to create MRF under block 2, level 10 and zone A', () => {
      createMRFPage.fillForm(1, '10', 'a', 0);
      createMRFPage.submitForm().then((resultDialogPresent: boolean) => {
        expect(resultDialogPresent).toBeTruthy();
        createMRFPage.getMRFNo().then((mrfNo: string) => {
          expect(mrfNo).toMatch('MRF-[0-9]+-[0-9]+$');
        });
        createMRFPage.getMaterialCount().then((materialCount: string) => {
          expect(materialCount).toEqual('0 materials');
        });
      });
    });

    it('should be able to create MRF under block 7, level 15 and zone C', () => {
      createMRFPage.fillForm(2, '15', 'C', 1);
      browser.sleep(10000);
      createMRFPage.submitForm().then((resultDialogPresent: boolean) => {
        expect(resultDialogPresent).toBeTruthy();
        createMRFPage.getMRFNo().then((mrfNo: string) => {
          expect(mrfNo).toMatch('MRF-[0-9]+-[0-9]+$');
        });
        createMRFPage.getMaterialCount().then((materialCount: string) => {
          expect(materialCount).toMatch('[0-9]+ materials$');
        });
        browser.actions().sendKeys(Key.ESCAPE).perform();
      });
    });
  });
});
