import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StatusChipComponent } from './status-chip.component';

describe('StatusChipComponent', () => {
  let fixture: ComponentFixture<StatusChipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [StatusChipComponent] }).compileComponents();
    fixture = TestBed.createComponent(StatusChipComponent);
  });

  it('renders an approved chip with the approved variant', () => {
    fixture.componentRef.setInput('status', 'Approved');
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement.querySelector('.chip');
    expect(el.textContent?.trim()).toBe('Approved');
    expect(el.classList).toContain('chip--approved');
  });

  it('falls back to the neutral variant for non-status text', () => {
    fixture.componentRef.setInput('status', 'Annual');
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement.querySelector('.chip');
    expect(el.classList).toContain('chip--neutral');
  });
});
