import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssuerComponent } from './issuer.component';

describe('IssuerComponent', () => {
  let component: IssuerComponent;
  let fixture: ComponentFixture<IssuerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IssuerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IssuerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
