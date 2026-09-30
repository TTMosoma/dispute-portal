import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

interface Dispute {
  id: string; transactionId: string; category: string;
  status: string; reason: string; createdAt: string; updatedAt: string;
}
interface History {
  fromStatus: string | null; toStatus: string;
  changedBy: string; note: string | null; changedAt: string;
}

@Component({
  selector: 'app-disputes',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './disputes.html',
  styleUrl: './disputes.css',
})
export class DisputesComponent implements OnInit {
  private http = inject(HttpClient);

  disputes = signal<Dispute[]>([]);
  loading = signal(true);
  expandedId = signal<string | null>(null);
  history = signal<History[]>([]);

  ngOnInit() {
    this.http.get<Dispute[]>('/api/disputes').subscribe({
      next: d => { this.disputes.set(d); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  toggle(id: string) {
    if (this.expandedId() === id) { this.expandedId.set(null); return; }
    this.expandedId.set(id);
    this.history.set([]);
    this.http.get<History[]>(`/api/disputes/${id}/history`).subscribe({
      next: h => this.history.set(h),
    });
  }

  withdraw(id: string) {
  if (!confirm('Withdraw this dispute?')) return;
  this.http.post(`/api/disputes/${id}/withdraw`, { note: null }).subscribe({
    next: () => this.ngOnInit(),          // reload the list to show new status
    error: () => alert('Could not withdraw.'),
  });
}

canWithdraw(status: string) {
  return status === 'Submitted' || status === 'UnderReview';
}
}
