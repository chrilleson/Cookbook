import { NextApiRequest, NextApiResponse } from "next"

import { ZodError } from "zod";
import { initTRPC } from "@trpc/server";
import superjson from "superjson";

type CreateContextOptions = {
  userAgent: string | undefined;
  host: string | undefined;
  res: NextApiResponse;
  req: NextApiRequest;
};

const createInnerTRPCContext = (opts: CreateContextOptions) => {
  return { ...opts };
};

export const createTRPCContext = (opts: CreateContextOptions) => {
  const userAgent = opts.req.headers["user-agent"] ?? undefined;
  const host = opts.req.headers["host"] ?? undefined;

  return createInnerTRPCContext({
    ...opts,
    userAgent,
    host,
  });
};

const t = initTRPC.context<typeof createTRPCContext>().create({
  transformer: superjson,
  errorFormatter({shape, error}) {
    return {
      ...shape,
      data: {
        ...shape.data,
        zodError: error.cause instanceof ZodError ? error.cause.flatten() : null,
      },
    };
  }
});

export const createTRPCRouter = t.router;

export const publicProcedure = t.procedure;