import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectAlternativeNameComponent } from './subject-alternative-name.component';

describe('SubjectAlternativeNameComponent', () => {
  let component: SubjectAlternativeNameComponent;
  let fixture: ComponentFixture<SubjectAlternativeNameComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubjectAlternativeNameComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectAlternativeNameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
