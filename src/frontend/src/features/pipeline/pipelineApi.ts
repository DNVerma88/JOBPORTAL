import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient } from '@/api/apiClient'

// ── Types ──────────────────────────────────────────────────────

export interface CandidatePipelineStage {
  id: string
  name: string
  order: number
  color: string
  candidateCount: number
}

export interface PipelineCandidate {
  id: string
  candidateName: string
  candidateEmail: string
  jobTitle: string
  appliedOn: string
  stageId: string
  avatarUrl?: string
}

export interface PipelineData {
  stages: CandidatePipelineStage[]
  candidates: PipelineCandidate[]
}

// ── Query keys ─────────────────────────────────────────────────

const KEYS = {
  pipeline: (jobId?: string) => ['recruiter', 'pipeline', jobId] as const,
}

// ── Hooks ──────────────────────────────────────────────────────

export function useHiringPipeline(jobId?: string) {
  return useQuery({
    queryKey: KEYS.pipeline(jobId),
    queryFn: async () => {
      const { data } = await apiClient.get<PipelineData>('/pipeline', { params: jobId ? { jobId } : undefined })
      return data
    },
  })
}

export function useMoveCandidateStage() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ candidateId, stageId, notes }: { candidateId: string; stageId: string; notes?: string }) =>
      apiClient.patch(`/pipeline/candidates/${candidateId}/stage`, { stageId, notes }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['recruiter', 'pipeline'] }),
  })
}

export function useCreatePipelineStage() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (body: { jobPostingId: string; name: string; color?: string; sortOrder: number; isDefault?: boolean }) =>
      apiClient.post('/pipeline/stages', body),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['recruiter', 'pipeline'] }),
  })
}

export function useUpdatePipelineStage() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ id, ...body }: { id: string; name: string; color?: string; sortOrder: number }) =>
      apiClient.put(`/pipeline/stages/${id}`, body),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['recruiter', 'pipeline'] }),
  })
}

export function useDeletePipelineStage() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => apiClient.delete(`/pipeline/stages/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['recruiter', 'pipeline'] }),
  })
}
